using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper.QueryableExtensions;

using Doitsu.Service.Core;
using Doitsu.Service.Core.Services.EmailService;
using Doitsu.Utils;
using Doitsu.Ecommerce.Core.ViewModels;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Optional;
using Optional.Async;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IOrderService : IBaseService<Orders>
    {
        Task<Option<string, string>> CheckoutCartAsync(CheckoutCartViewModel data, EcommerceIdentityUser user);
        Task<ImmutableList<OrderViewModel>> GetAllOrdersByUserIdAsync(int userId);
        Task<OrderDetailViewModel> GetOrderDetailByCodeAsync(string orderCode);
        Task<ImmutableList<OrderDetailViewModel>> GetOrderDetailByParams(OrderStatusEnum? orderStatus,
                                                                         DateTime? fromDate,
                                                                         DateTime? toDate,
                                                                         string userPhone,
                                                                         string orderCode);
        Task<Option<OrderViewModel, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum);
    }

    public class OrderService : BaseService<Orders>, IOrderService
    {
        private readonly IEmailService emailService;

        public OrderService(EcommerceDbContext dbContext,
                            IMapper mapper,
                            ILogger<BaseService<Orders, EcommerceDbContext>> logger,
                            IEmailService emailService) : base(dbContext, mapper, logger)
        {
            this.emailService = emailService;
        }

        private string GenerateOrderCode()
        {
            var currentDate = DateTime.Now;
            var year = currentDate.Year.ToString();
            var orderCode = DataUtils.GenerateOrderCode(6, new Random());
            var orderCodeFirstPart = orderCode.Substring(0, 3);
            var orderCodeSecondPart = orderCode.Substring(3);
            var yearFirstPart = year.Substring(0, 2);
            var yearSecondPart = year.Substring(2);
            var result = $"{orderCodeFirstPart}{yearFirstPart}{orderCodeSecondPart}{yearSecondPart}";
            return result;
        }

        public async Task<Option<string, string>> CheckoutCartAsync(CheckoutCartViewModel data, EcommerceIdentityUser user)
        {
            return await new { data, user }
                .SomeNotNull()
                .WithException(string.Empty)
                .FlatMapAsync(async d =>
                {
                    using (var trans = await CreateTransactionAsync())
                    {

                        var order = new Orders();
                        order.Status = (int)OrderStatusEnum.New;
                        order.Discount = 0;
                        order.Code = GenerateOrderCode().ToUpper();
                        order.TotalPrice = data.TotalPrice;
                        order.FinalPrice = data.TotalPrice;
                        order.TotalQuantity = data.TotalQuantity;
                        order.UserId = user.Id;

                        foreach (var item in data.CartItems)
                        {
                            var orderItem = new OrderItems
                            {
                                Discount = 0,
                                ProductId = item.Id,
                                OrderId = order.Id,
                                SubTotalPrice = item.SubTotal,
                                SubTotalQuantity = item.SubQuantity,
                                SubTotalFinalPrice = item.SubTotal
                            };
                            if (order.OrderItems == null) order.OrderItems = new List<OrderItems>();
                            order.OrderItems.Add(orderItem);
                        }
                        await CreateAsync(order);
                        await CommitAsync();
                        trans.Commit();
                        try
                        {
                            var messagePayloads = new List<MessagePayload>()
                            {
                                await emailService.PrepareLeaderOrderMailConfirmAsync(user, order),
                                await emailService.PrepareCustomerOrderMailConfirm(user, order)
                            };

                            var emailResult = await emailService.SendEmailWithBachMocWrapperAsync(messagePayloads);
                            emailResult.MatchNone(error =>
                            {
                                Logger.LogInformation("Send mails to confirm order code {Code} on {CreatedDate} failure: {error}", user.Id, order.Code, order.CreatedDate, error);
                            });

                            Logger.LogInformation("User {Id} completed order code {Code} on {CreatedDate}", user.Id, order.Code, order.CreatedDate);
                            return Option.Some<string, string>(order.Code);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Process user order fail");
                            return Option.None<string, string>($"Quá trình xử lý đơn hàng bị lỗi.");
                        }
                    }
                });
        }

        public async Task<ImmutableList<OrderViewModel>> GetAllOrdersByUserIdAsync(int userId)
        {
            var result = await this.GetAsNoTracking(x => x.UserId.Equals(userId))
                .ProjectTo<OrderViewModel>(Mapper.ConfigurationProvider)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return result.ToImmutableList();
        }

        public async Task<OrderDetailViewModel> GetOrderDetailByCodeAsync(string orderCode)
        {
            var result = await this.FirstOrDefaultAsync<OrderDetailViewModel>(x => x.Code.Equals(orderCode));
            return result;

        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum)
        {
            return await (orderId, statusEnum).SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async d => await this.SelfRepository.AnyAsync(x => x.Id == d.orderId), "Không thể tim thấy đơn hàng cần đổi trạng thái")
                .MapAsync(async d =>
                {
                    var order = await this.FindByKeysAsync(d.orderId);
                    order.Status = (int)statusEnum;
                    this.Update(order);
                    await CommitAsync();
                    return Mapper.Map<OrderViewModel>(order);
                });
        }

        public async Task<ImmutableList<OrderDetailViewModel>> GetOrderDetailByParams(OrderStatusEnum? orderStatus,
                                                                                      DateTime? fromDate,
                                                                                      DateTime? toDate,
                                                                                      string userPhone,
                                                                                      string orderCode)
        {
            var query = this.GetAllAsNoTracking();

            if (orderStatus.HasValue)
            {
                query = query.Where(x => x.Status == (int)orderStatus.Value);
            }

            if (!userPhone.IsNullOrEmpty())
            {
                query = query.Where(x => x.User.PhoneNumber.Contains(userPhone));
            }

            if (fromDate.HasValue && fromDate != DateTime.MinValue)
            {
                query = query.Where(x => x.CreatedDate >= fromDate.Value.StartOfDay());
                if (toDate.HasValue && toDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreatedDate <= toDate.Value.EndOfDay());
                }
            }

            if (!orderCode.IsNullOrEmpty())
            {
                query = query.Where(x => x.Code.Contains(orderCode));
            }

            var result = await query
                .ProjectTo<OrderDetailViewModel>(Mapper.ConfigurationProvider)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return result.ToImmutableList();
        }
    }
}