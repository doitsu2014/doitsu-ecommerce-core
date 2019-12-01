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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Async;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.IdentitiesExtension;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IOrderService : IBaseService<Orders>
    {
        /// <summary>
        /// Version on Bach Moc Website
        /// Should be update
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Option<string, string>> CheckoutCartAsync(CheckoutCartViewModel data, EcommerceIdentityUser user);
        Task<ImmutableList<OrderViewModel>> GetAllOrdersByUserIdAsync(int userId);
        Task<OrderDetailViewModel> GetOrderDetailByCodeAsync(string orderCode);
        Task<ImmutableList<OrderDetailViewModel>> GetOrderDetailByParams(OrderStatusEnum? orderStatus,
                                                                         DateTime? fromDate,
                                                                         DateTime? toDate,
                                                                         string userPhone,
                                                                         string orderCode);
        Task<Option<OrderViewModel, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum);
        Task<Option<OrderViewModel, string>> CreateOrderWithOptionAsync(CreateOrderWithOptionViewModel request);
    }

    public class OrderService : BaseService<Orders>, IOrderService
    {
        private readonly IEmailService emailService;
        private readonly IProductService productService;
        private readonly EcommerceIdentityUserManager<EcommerceIdentityUser> userManager;

        public OrderService(EcommerceDbContext dbContext,
                            IMapper mapper,
                            ILogger<BaseService<Orders, EcommerceDbContext>> logger,
                            IEmailService emailService,
                            IProductService productService,
                            EcommerceIdentityUserManager<EcommerceIdentityUser> userManager) : base(dbContext, mapper, logger)
        {
            this.emailService = emailService;
            this.productService = productService;
            this.userManager = userManager;
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

        public async Task<Option<OrderViewModel, string>> CreateOrderWithOptionAsync(CreateOrderWithOptionViewModel request)
        {
            return await request.SomeNotNull()
                .WithException("Dữ liệu truyền lên rỗng.")
                .Filter(d => d.UserId > 0, "Không tìm thấy thông tin người đặt hàng.")
                .FlatMapAsync(async d => await MappingFromOrderWithOptionToOrder(d))
                .MapAsync(async o =>
                {
                    await this.CreateAsync(o);
                    await this.CommitAsync();
                    return o;
                })
                .MapAsync(async o =>
                {
                    try
                    {
                        var user = await userManager.FindByIdAsync(o.UserId.ToString());
                        var messagePayloads = new List<MessagePayload>()
                        {
                            await emailService.PrepareLeaderOrderMailConfirmAsync(user, o),
                            await emailService.PrepareCustomerOrderMailConfirm(user, o)
                        };

                        var emailResult = await emailService.SendEmailWithBachMocWrapperAsync(messagePayloads);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"Process email for order {o.Code} failure.");
                    }
                    return this.Mapper.Map<OrderViewModel>(o);
                });
        }

        private async Task<Option<Orders, string>> MappingFromOrderWithOptionToOrder(CreateOrderWithOptionViewModel request)
        {
            return await request.SomeNotNull()
                .WithException("Thông tin đơn hàng rỗng")
                .MapAsync(async d =>
                {
                    foreach (var orderItem in d.OrderItems)
                    {
                        var productVariant = await productService.FindProductVariantFromOptionsAsync(orderItem.ProductOptionValues);
                        if (productVariant != null)
                        {
                            orderItem.ProductId = productVariant.ProductId;
                            if (productVariant.PromotionDetails != null && productVariant.PromotionDetails.Count > 0)
                            {
                                orderItem.Discount = productVariant.PromotionDetails.OrderByDescending(x => x.Id).First().DiscountPercent;
                            }
                            var subTotalPrice = orderItem.SubTotalFinalPrice;
                            var subTotalQuantity = orderItem.SubTotalQuantity;
                            var discount = orderItem.Discount ?? 0;
                            var price = subTotalPrice * subTotalQuantity;
                            orderItem.SubTotalFinalPrice = price - (price * ((decimal)discount) / 100);
                            orderItem.ProductId = productVariant.ProductId;
                            orderItem.ProductVariantId = productVariant.Id;
                            d.TotalPrice += price;
                            d.TotalQuantity += orderItem.SubTotalQuantity;
                            d.FinalPrice += orderItem.SubTotalFinalPrice;
                        }
                    }

                    if (d.Priority.HasValue)
                    {
                        var originPrice = d.TotalPrice * d.TotalQuantity;
                        var priorityValue = ((decimal)d.Priority.Value / 100);
                        d.FinalPrice += (originPrice * priorityValue);
                    }

                    var order = this.Mapper.Map<Orders>(request);
                    order.Code = DataUtils.GenerateOrderCode(Constants.OrderInformation.ORDER_CODE_LENGTH, new Random());
                    order.CreatedDate = DateTime.UtcNow.ToVietnamDateTime();

                    return order;
                });
        }
    }
}