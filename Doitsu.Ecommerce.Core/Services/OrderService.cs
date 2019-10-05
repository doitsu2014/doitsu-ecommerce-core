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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService emailService;
        private readonly LeaderMail leaderMailOption;

        public OrderService(
            IUnitOfWork unitOfWork,
            ILogger<BaseService<Orders>> logger,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService,
            IOptionsMonitor<LeaderMail> leaderMailOption) : base(unitOfWork, logger)
        {
            _httpContextAccessor = httpContextAccessor;
            this.emailService = emailService;
            this.leaderMailOption = leaderMailOption.CurrentValue;
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
                    using (var trans = await UnitOfWork.CreateTransactionAsync())
                    {
                        try
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
                                order.OrderItems.Add(orderItem);
                            }
                            await this.CreateAsync(order);
                            await this.UnitOfWork.CommitAsync();
                            trans.Commit();
                            var messagePayloads = new List<MessagePayload>()
                            {
                                await PrepareLeaderOrderMailConfirmAsync(user, order),
                                await PrepareCustomerOrderMailConfirm(user, order),
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
                            trans.Rollback();
                            return Option.None<string, string>($"Quá trình xử lý đơn hàng bị lỗi.");
                        }
                    }
                });
        }

        private async Task<MessagePayload> PrepareCustomerOrderMailConfirm(EcommerceIdentityUser user, Orders order)
        {
            var brandService = this.UnitOfWork.GetService<IBrandService>();
            try
            {
                var currentBrand = await brandService.FirstOrDefaultActiveAsync();
                var subject = $"[{currentBrand.Name}] XÁC NHẬN ĐƠN HÀNG #{order.Code} - {DateTime.UtcNow.ToVietnamDateTime().ToShortDateString()}";
                var destPayload = new MailPayloadInformation
                {
                    Mail = user.Email,
                    Name = user.Fullname
                };

                var body = $"<p>Bạn đã đặt thành công đơn hàng có mã đơn: <a href='{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/nguoi-dung/thong-tin-tai-khoan'>#{order.Code}</a></p>";
                body += $"<p>Để có thể xem chi tiết đơn hàng, mong quý khách nhấp vào đường dẫn phía trên.</p><br/>";

                var messagePayload = new MessagePayload();
                messagePayload.Subject = subject;
                messagePayload.Body = body;
                messagePayload.DestEmail = destPayload;
                return messagePayload;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Cannot send email to confirm Order Code {order.Code} of {user.Fullname} with id {user.Id}");
                return null;
            }
        }

        private async Task<MessagePayload> PrepareLeaderOrderMailConfirmAsync(EcommerceIdentityUser user, Orders order)
        {
            var brandService = this.UnitOfWork.GetService<IBrandService>();
            try
            {
                var currentBrand = await brandService.FirstOrDefaultActiveAsync();
                var subject = $"[{currentBrand.Name}] ĐƠN HÀNG MỚI #{order.Code} - {DateTime.UtcNow.ToVietnamDateTime().ToShortDateString()}";
                var destPayload = new MailPayloadInformation
                {
                    Mail = leaderMailOption.Mail,
                    Name = leaderMailOption.Name
                };
                var body = "<p>";
                body += "Có đơn đặt hàng vào hệ thống. Thông tin chi tiết:<br/>";
                body += $"Người đặt: {user.Fullname}<br/>";
                body += $"Email: {user.Email}<br/>";
                body += $"Số điện thoại: {user.PhoneNumber}<br/>";
                body += $"Địa chỉ: {user.Address}<br/>";
                body += $"Mã đơn: #{order.Code}<br/>";
                body += $"Ngày đặt: {DateTime.UtcNow.ToVietnamDateTime().ToShortDateString()}<br/>";
                body += $"Tổng tiền: {order.FinalPrice}";
                body += "</p>";
                body += $"<p>Hãy vào trang quản lý để xem thông tin chi tiết: <a href='{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Admin'>Trang Quản Lý</a></p><br/>";

                var messagePayload = new MessagePayload();
                messagePayload.Subject = subject;
                messagePayload.Body = body;
                messagePayload.DestEmail = destPayload;

                return messagePayload;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Cannot send email to confirm Order Code {order.Code} of {user.Fullname} with id {user.Id}");
                return null;
            }
        }

        public async Task<ImmutableList<OrderViewModel>> GetAllOrdersByUserIdAsync(int userId)
        {
            var result = await this.GetActiveAsNoTracking(x => x.UserId.Equals(userId))
                .ProjectTo<OrderViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return result.ToImmutableList();
        }

        public async Task<OrderDetailViewModel> GetOrderDetailByCodeAsync(string orderCode)
        {
            var result = await this.FirstOrDefaultActiveAsync<OrderDetailViewModel>(x => x.Code.Equals(orderCode));
            return result;

        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum)
        {
            return await (orderId, statusEnum).SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async d => await this.SelfDbSet.AnyAsync(x => x.Id == d.orderId), "Không thể tim thấy đơn hàng cần đổi trạng thái")
                .MapAsync(async d =>
                {
                    var order = await this.FindByKeysAsync(d.orderId);
                    order.Status = (int)statusEnum;
                    this.Update(order);
                    await this.UnitOfWork.CommitAsync();
                    return this.UnitOfWork.Mapper.Map<OrderViewModel>(order);
                });
        }

        public async Task<ImmutableList<OrderDetailViewModel>> GetOrderDetailByParams(OrderStatusEnum? orderStatus,
                                                                                      DateTime? fromDate,
                                                                                      DateTime? toDate,
                                                                                      string userPhone,
                                                                                      string orderCode)
        {
            var query = this.GetAllActiveAsNoTracking();

            if(orderStatus.HasValue) {
                query = query.Where(x => x.Status == (int) orderStatus.Value);
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

            if(!orderCode.IsNullOrEmpty())
            {
                query = query.Where(x => x.Code.Contains(orderCode));
            }

            var result = await query
                .ProjectTo<OrderDetailViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            
            return result.ToImmutableList();
        }
    }
}