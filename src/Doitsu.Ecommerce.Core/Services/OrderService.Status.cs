using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
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

                        var userTransaction = this.userTransactionService.PrepareUserTransaction(this.Mapper.Map<OrderDetailViewModel>(order), ImmutableList<ProductVariantViewModel>.Empty, order.UserId, UserTransactionTypeEnum.Rollback);
                        await this.userTransactionService.UpdateUserBalanceAsync(userTransaction, order.UserId);
                        await userTransactionService.CreateAsync(userTransaction);
                        return Option.Some<OrderViewModel, string>(Mapper.Map<OrderViewModel>(order));
                    }
                });
        }

        public async Task<Option<OrderViewModel, string>> CompleteOrderAsync(string orderCode, int userId, string note = "")
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                return await (orderCode).SomeNotNull()
                    .WithException("Mã của đơn không hợp lệ.")
                    .FlatMapAsync(async req =>
                    {
                        var auditUser = await userManager.FindByIdAsync(userId.ToString());
                        var order = await this.FirstOrDefaultAsync(o => o.Code == orderCode && OrderTypeEnum.Summary != o.Type && o.Status != (int)OrderStatusEnum.Cancel);
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
                    })
                    .MapAsync(async res => {
                        await transaction.CommitAsync();
                        return res;
                    });
            }
        }


        public async Task<Option<OrderViewModel, string>> ChangeStatusToProcessOrderAsync(string orderCode, int userId)
        {
            return await (orderCode).SomeNotNull()
                .WithException("Mã của Đơn Tổng không hợp lệ.")
                .FlatMapAsync(async req =>
                {
                    var order = await this.GetAsTracking(o => o.Code == orderCode && OrderTypeEnum.Summary != o.Type).FirstOrDefaultAsync();
                    var auditUser = await userManager.FindByIdAsync(userId.ToString());
                    if (auditUser == null)
                    {
                        return Option.None<OrderViewModel, string>("Tài khoản đang thao tác xử lý đơn hàng không tồn tại hoặc bị xóa.");
                    }
                    var isInRoleAdmin = await userManager.IsInRoleAsync(auditUser, Constants.UserRoles.ADMIN);
                    if (order == null) return Option.None<OrderViewModel, string>("Không tìm thấy đơn hàng phù hợp");
                    else if (!isInRoleAdmin) return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} không thể xử lý do người thao tác không phải là Admin.");
                    else
                    {
                        order.Status = (int)OrderStatusEnum.Processing;
                        this.Update(order);
                        await this.CommitAsync();
                        return Option.Some<OrderViewModel, string>(this.Mapper.Map<OrderViewModel>(order));
                    }
                });
        }

        public async Task<Option<OrderViewModel, string>> ChangeStatusToDeliveryOrderAsync(string orderCode, int userId, bool isWorkingOnInventoryQuantity = false)
        {
            return await (orderCode).SomeNotNull()
                .WithException("Mã của Đơn Tổng không hợp lệ.")
                .FlatMapAsync(async req =>
                {
                    var order = await this.FirstOrDefaultAsync(o => o.Code == orderCode && OrderTypeEnum.Summary != o.Type, o => o.Include(qO => qO.OrderItems), isTracking: true);
                    var auditUser = await userManager.FindByIdAsync(userId.ToString());
                    if (auditUser == null)
                    {
                        return Option.None<OrderDetailViewModel, string>("Tài khoản đang thao tác xử lý đơn hàng không tồn tại hoặc bị xóa.");
                    }
                    var isInRoleAdmin = await userManager.IsInRoleAsync(auditUser, Constants.UserRoles.ADMIN);
                    if (order == null) return Option.None<OrderDetailViewModel, string>("Không tìm thấy đơn hàng phù hợp");
                    else if (!isInRoleAdmin) return Option.None<OrderDetailViewModel, string>($"Đơn hàng {orderCode} không thể xử lý do người thao tác không phải là Admin.");
                    else
                    {
                        order.Status = (int)OrderStatusEnum.Delivery;
                        this.Update(order);
                        await this.CommitAsync();
                        return Option.Some<OrderDetailViewModel, string>(this.Mapper.Map<OrderDetailViewModel>(order));
                    }
                })
                 .FlatMapAsync(async req =>
                 {
                     if (isWorkingOnInventoryQuantity)
                     {
                         var orderItems = req.OrderItems;

                         // TODO: Work with normal product
                         var orderedProductIds = orderItems.Where(oi => oi.ProductVariantId == null).Select(oi => oi.ProductId);

                         var orderedPvIds = orderItems.Where(oi => oi.ProductVariantId != null).Select(oi => oi.ProductVariantId ?? 0).ToArray();
                         return await this.productVariantService.DecreaseBatchPvInventoryQuantityAsync(orderedPvIds, 1)
                             .MapAsync(
                                 async ids => await Task.FromResult(this.Mapper.Map<OrderViewModel>(req))
                             );
                     }
                     return Option.Some<OrderViewModel, string>(this.Mapper.Map<OrderViewModel>(req));
                 }); ;
        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum, int auditUserId, string note = "", bool isWorkingOnInventory = false)
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
                            return await CompleteOrderAsync(orderCode, auditUserId, note);
                        case OrderStatusEnum.Processing:
                            return await ChangeStatusToProcessOrderAsync(orderCode, auditUserId);
                        case OrderStatusEnum.Delivery:
                            return await ChangeStatusToDeliveryOrderAsync(orderCode, auditUserId, isWorkingOnInventoryQuantity: isWorkingOnInventory);
                        default:
                            return Option.None<OrderViewModel, string>("Không thể chuyển đơn hàng sang trạng thái này.");
                    }
                });
        }
    }
}