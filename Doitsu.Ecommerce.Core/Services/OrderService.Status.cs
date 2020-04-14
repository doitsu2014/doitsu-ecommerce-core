using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.Core.Services
{
    public partial class OrderService
    {
        public async Task<Option<OrderViewModel, string>> CancelOrderAsync(string orderCode, int auditUserId, string cancelNote = "")
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                return await CancelOrderInternalAsync(orderCode, auditUserId, cancelNote)
                    .MapAsync(async res =>
                    {
                        await this.CommitAsync();
                        await transaction.CommitAsync();
                        return res;
                    });
            }

        }

        /// <summary>
        /// You should commit when using this method.
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="auditUserId"></param>
        /// <param name="cancelNote"></param>
        /// <returns></returns>
        private async Task<Option<OrderViewModel, string>> CancelOrderInternalAsync(string orderCode, int auditUserId, string cancelNote = "")
        {
            return await (orderCode, auditUserId).SomeNotNull()
                .WithException(string.Empty)
                .Filter(d => !d.orderCode.IsNullOrEmpty(), "Mã đơn hàng gửi lên rỗng.")
                .FlatMapAsync(async d =>
                {
                    var order = await Get(o => o.Code == d.orderCode && OrderTypeEnum.Summary != o.Type).FirstOrDefaultAsync();
                    var auditUser = await userManager.FindByIdAsync(auditUserId.ToString());
                    if (auditUser == null)
                    {
                        return Option.None<OrderViewModel, string>("Tài khoản đang thao tác hủy đơn hàng không tồn tại hoặc bị xóa.");
                    }
                    var isInRoleAdmin = await userManager.IsInRoleAsync(auditUser, Constants.UserRoles.ADMIN);
                    if (order == null)
                    {
                        return Option.None<OrderViewModel, string>("Không tìm thấy đơn hàng phù hợp để hủy đơn.");
                    }
                    else if (!isInRoleAdmin && (OrderStatusEnum)order.Status != OrderStatusEnum.New)
                    {
                        return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} không phải là đơn hàng MỚI nên không thể hủy.");
                    }
                    else if ((OrderStatusEnum)order.Status == OrderStatusEnum.Done)
                    {
                        return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} đã HOÀN THÀNH nên không thể hủy.");
                    }
                    else if ((OrderStatusEnum)order.Status == OrderStatusEnum.Cancel)
                    {
                        return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} đã được HỦY nên không thể hủy.");
                    }
                    else
                    {
                        order.Status = (int)OrderStatusEnum.Cancel;
                        order.CancelNote = $"{cancelNote}.";
                        this.Update(order);

                        var orderOwner = await userManager.FindByIdAsync(order.UserId.ToString());
                        var userTransaction = this.userTransactionService.PrepareUserTransaction(order, ImmutableList<ProductVariantViewModel>.Empty, orderOwner, UserTransactionTypeEnum.Rollback);
                        await this.userTransactionService.UpdateUserBalanceAsync(userTransaction, orderOwner);
                        await userTransactionService.CreateAsync(userTransaction);
                        return Option.Some<OrderViewModel, string>(Mapper.Map<OrderViewModel>(order));
                    }
                });
        }

        public async Task<Option<OrderViewModel, string>> CompleteOrderAsync(string orderCode, int userId, string note = "")
        {
            return await (orderCode).SomeNotNull()
                .WithException("Mã của Đơn Tổng không hợp lệ.")
                .FlatMapAsync(async req =>
                {
                    var order = await this.GetAsTracking(o => o.Code == orderCode && OrderTypeEnum.Summary != o.Type).Where(o => o.Status != (int)OrderStatusEnum.Cancel).FirstOrDefaultAsync();
                    var auditUser = await userManager.FindByIdAsync(userId.ToString());
                    if (auditUser == null)
                    {
                        return Option.None<OrderViewModel, string>("Tài khoản đang thao tác hủy đơn hàng không tồn tại hoặc bị xóa.");
                    }
                    var isInRoleAdmin = await userManager.IsInRoleAsync(auditUser, Constants.UserRoles.ADMIN);
                    if (order == null) return Option.None<OrderViewModel, string>("Không tìm thấy đơn hàng phù hợp");
                    else if (!isInRoleAdmin) return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} không thể hoàn thành do người thao tác không phải là Admin.");
                    else if (order.Status == (int)OrderStatusEnum.Cancel) return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} đã HỦY nên không thể hoàn thành.");
                    else
                    {
                        order.Status = (int)OrderStatusEnum.Done;
                        if (note.IsNotNullOrEmpty())
                        {
                            order.Note = note;
                        }
                        this.Update(order);
                        await this.CommitAsync();
                        return Option.Some<OrderViewModel, string>(this.Mapper.Map<OrderViewModel>(order));
                    }
                });
        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum, int auditUserId, string note = "")
        {
            return await (orderId, statusEnum).SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async d => await this.SelfRepository.AnyAsync(x => x.Id == d.orderId), "Không thể tim thấy đơn hàng cần đổi trạng thái")
                .FlatMapAsync(async d =>
                {
                    var orderCode = await this.Get(o => o.Id == orderId).Select(o => o.Code).FirstOrDefaultAsync();
                    switch (statusEnum)
                    {
                        case OrderStatusEnum.Cancel:
                            return await CancelOrderAsync(orderCode, auditUserId, note);
                        case OrderStatusEnum.Done:
                            return await CompleteOrderAsync(orderCode, auditUserId);
                        default:
                            return Option.None<OrderViewModel, string>("Không thể chuyển đơn hàng sang trạng thái này.");
                    }
                });
        }
    }
}