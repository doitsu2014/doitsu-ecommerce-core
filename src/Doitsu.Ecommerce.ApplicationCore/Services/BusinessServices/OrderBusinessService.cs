using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Models.ExportModels;
using Doitsu.Ecommerce.ApplicationCore.Models.ViewModels;
using Doitsu.Ecommerce.ApplicationCore.Services.IdentityManagers;
using Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Async;
using Optional.Collections;

namespace Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices
{
    public partial class OrderBusinessService : IOrderBusinessService
    {
        private readonly ILogger<OrderBusinessService> logger;
        private readonly IBaseEcommerceRepository<Orders> orderRepository;
        private readonly IBaseEcommerceRepository<Products> productRepository;
        private readonly IBaseEcommerceRepository<ProductVariants> productVariantRepository;
        private readonly IBaseEcommerceRepository<UserTransaction> userTransactionRepository;
        private readonly EcommerceIdentityUserManager<EcommerceIdentityUser> userManager;
        private readonly IEcommerceDatabaseManager databaseManager;

        public OrderBusinessService(ILogger<OrderBusinessService> logger,
                                    IEcommerceDatabaseManager databaseManager,
                                    IBaseEcommerceRepository<Orders> orderRepository,
                                    IBaseEcommerceRepository<Products> productRepository,
                                    IBaseEcommerceRepository<ProductVariants> productVariantRepository,
                                    IBaseEcommerceRepository<UserTransaction> userTransactionRepository)
        {
            this.logger = logger;
            this.databaseManager = databaseManager;
            this.orderRepository = orderRepository;
            this.userTransactionRepository = userTransactionRepository;
            this.productRepository = productRepository;
            this.productVariantRepository = productVariantRepository;
        }

        public async Task<Option<Orders, string>> CancelOrderAsync(string orderCode, int auditUserId, string cancelNote = "")
        {
            return await (orderCode, auditUserId).SomeNotNull()
                .WithException(string.Empty)
                .Filter(d => !d.orderCode.IsNullOrEmpty(), "Mã đơn hàng gửi lên rỗng.")
                .FlatMapAsync(async d =>
                {
                    var order = await this.orderRepository.FirstOrDefaultAsync(new OrderFilterByOrderCodeSpecification(d.orderCode));
                    var auditUser = await this.userManager.FindByIdAsync(auditUserId.ToString());
                    if (auditUser == null)
                    {
                        return Option.None<Orders, string>("Tài khoản đang thao tác hủy đơn hàng không tồn tại hoặc bị xóa.");
                    }
                    var isInRoleAdmin = await this.userManager.IsInRoleAsync(auditUser, Constants.UserRoles.ADMIN);
                    if (order == null)
                    {
                        return Option.None<Orders, string>("Không tìm thấy đơn hàng phù hợp để hủy đơn.");
                    }
                    else if (!isInRoleAdmin && (OrderStatusEnum)order.Status != OrderStatusEnum.New)
                    {
                        return Option.None<Orders, string>($"Đơn hàng {orderCode} không phải là đơn hàng MỚI nên không thể hủy.");
                    }
                    else if ((OrderStatusEnum)order.Status == OrderStatusEnum.Done)
                    {
                        return Option.None<Orders, string>($"Đơn hàng {orderCode} đã HOÀN THÀNH nên không thể hủy.");
                    }
                    else if ((OrderStatusEnum)order.Status == OrderStatusEnum.Cancel)
                    {
                        return Option.None<Orders, string>($"Đơn hàng {orderCode} đã được HỦY nên không thể hủy.");
                    }
                    else
                    {
                        order.Status = (int)OrderStatusEnum.Cancel;
                        order.CancelNote = $"{cancelNote}.";
                        await orderRepository.UpdateAsync(order);
                        return Option.Some<Orders, string>(order);
                    }
                });
        }

        public async Task<Option<Orders, string>> CancelSummaryOrderAsync(int summaryOrderId, int auditUserId, string cancelNote = "")
        {
            return await (summaryOrderId, auditUserId, cancelNote).SomeNotNull()
                .WithException("Mã của Đơn Tổng không hợp lệ.")
                .MapAsync(async req =>
                {
                    var summaryOrder = await this.orderRepository.FirstOrDefaultAsync(new OrderSummaryFilterByIdSpecification(req.summaryOrderId));
                    summaryOrder.Status = (int)OrderStatusEnum.Cancel;
                    summaryOrder.CancelNote = $"{cancelNote}";
                    await this.orderRepository.UpdateAsync(summaryOrder);
                    return (summaryOrder, req.auditUserId, req.cancelNote);
                })
                .FlatMapAsync(async d =>
                {
                    var listSaleOrderCode = d.summaryOrder.InverseSummaryOrders
                            .Where(io => io.Status != (int)OrderStatusEnum.Done)
                            .Select(io => io.Code)
                            .ToImmutableList();

                    var listResult = new List<Option<Orders, string>>();
                    foreach (var updatingOrderCode in listSaleOrderCode)
                    {
                        listResult.Add(await CancelOrderAsync(updatingOrderCode, d.auditUserId, d.cancelNote));
                    }
                    if (!listResult.All(r => r.HasValue))
                    {
                        return Option.None<Orders, string>(listResult.Exceptions().Aggregate((a, b) => $"{a},\n{b}"));
                    }

                    return Option.Some<Orders, string>(d.summaryOrder);
                });
        }

        public async Task<Option<Orders, string>> ChangeOrderCancelNote(int orderId, string note = "")
        {
            return await (orderId, note)
                .SomeNotNull()
                .WithException(string.Empty)
                .MapAsync(async d => (order: await this.orderRepository.FirstOrDefaultAsync(new OrderFilterByIdSpecification(d.orderId)), d.note))
                .FilterAsync(async d => await Task.FromResult(d.order != null), "Không tìm thấy đơn hàng cần thay đổi")
                .MapAsync(async data =>
                {
                    data.order.CancelNote = note;
                    await this.orderRepository.UpdateAsync(data.order);
                    return data.order;
                });
        }

        public async Task<Option<Orders, string>> ChangeOrderDeliveryProviderCodeAsync(int orderId, string code)
        {
            return await (orderId, code)
                    .SomeNotNull()
                    .WithException(string.Empty)
                    .Filter(data => !data.code.IsNullOrEmpty(), "Phải nhập mã vận chuyển")
                    .MapAsync(async d => (order: await this.orderRepository.FirstOrDefaultAsync(new OrderFilterByIdSpecification(d.orderId)), d.code))
                    .FilterAsync(async data => await Task.FromResult(data.order != null), "Không tìm thấy đơn hàng cần thay đổi")
                    .MapAsync(async data =>
                    {
                        data.order.DeliveryProviderCode = data.code;
                        await this.orderRepository.UpdateAsync(data.order);
                        return data.order;
                    });
        }

        public async Task<Option<Orders, string>> ChangeOrderNote(int orderId, string note = "")
        {
            return await (orderId, note)
                .SomeNotNull()
                .WithException(string.Empty)
                .MapAsync(async d => (order: await this.orderRepository.FirstOrDefaultAsync(new OrderFilterByIdSpecification(d.orderId)), d.note))
                .FilterAsync(async d => await Task.FromResult(d.order != null), "Không tìm thấy đơn hàng cần thay đổi")
                .MapAsync(async data =>
                {
                    data.order.Note = note;
                    await this.orderRepository.UpdateAsync(data.order);
                    return data.order;
                });
        }

        public async Task<Option<Orders, string>> ChangeOrderPaymentProofImageUrlAsync(int orderId, string proof = "")
        {
            return await (orderId, proof)
                .SomeNotNull()
                .WithException(string.Empty)
                .MapAsync(async d => (order: await this.orderRepository.FirstOrDefaultAsync(new OrderFilterByIdSpecification(d.orderId)), d.proof))
                .FilterAsync(async d => await Task.FromResult(d.order != null), "Không tìm thấy đơn hàng cần thay đổi")
                .MapAsync(async data =>
                {
                    data.order.PaymentProofImageUrl = data.proof;
                    await this.orderRepository.UpdateAsync(data.order);
                    return data.order;
                });
        }

        public async Task<Option<Orders, string>> ChangeOrderPaymentValueAsync(int orderId, decimal? paymentValue)
        {
            return await (orderId, paymentValue)
                 .SomeNotNull()
                 .WithException(string.Empty)
                 .Filter(data => data.paymentValue != null, "Phải nhập giá trị thanh toán")
                 .MapAsync(async d => (order: await this.orderRepository.FirstOrDefaultAsync(new OrderFilterByIdSpecification(d.orderId)), d.paymentValue))
                 .FilterAsync(async d => await Task.FromResult(d.order != null), "Không tìm thấy đơn hàng cần thay đổi")
                 .MapAsync(async data =>
                 {
                     data.order.PaymentValue = data.paymentValue.Value;
                     await this.orderRepository.UpdateAsync(data.order);
                     return data.order;
                 });
        }

        public async Task<Option<Orders, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum, int auditUserId, string note = "", bool isWorkingInventoryQuantity = false) => await (orderId, auditUserId, statusEnum, note, isWorkingInventoryQuantity).SomeNotNull()
            .WithException(string.Empty)
            .FilterAsync(async d => await this.orderRepository.AnyAsync(new OrderFilterByIdSpecification(d.orderId)), "Không thể tim thấy đơn hàng cần đổi trạng thái")
            .FlatMapAsync(async d =>
            {
                var orderCode = (await this.orderRepository.ListAsync(new OrderFilterByIdSpecification(d.orderId))).Select(o => o.Code).FirstOrDefault();
                switch (statusEnum)
                {
                    case OrderStatusEnum.Cancel:
                        return await CancelOrderAsync(orderCode, d.auditUserId, d.note);
                    case OrderStatusEnum.Done:
                        return await CompleteOrderAsync(orderCode, d.auditUserId, d.note);
                    case OrderStatusEnum.Processing:
                        return await ChangeStatusToProcessOrderAsync(orderCode, d.auditUserId);
                    case OrderStatusEnum.Delivery:
                        return await ChangeStatusToDeliveryOrderAsync(orderCode, d.auditUserId);
                    default:
                        return Option.None<Orders, string>("Không thể chuyển đơn hàng sang trạng thái này.");
                }
            });

        public async Task<Option<Orders, string>> ChangeStatusToDeliveryOrderAsync(string orderCode, int userId)
        {
            return await (await (orderCode, userId)
                .SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => req.orderCode.IsNotNullOrEmpty(), "Mã của Đơn Tổng không hợp lệ.")
                .Filter(req => req.userId > 0, "Id của người dùng không hợp lệ.")
                .MapAsync(async req => (
                    order: await this.orderRepository.FirstOrDefaultAsync(new OrderFilterByOrderCodeSpecification(req.orderCode)),
                    auditUser: await userManager.FindByIdAsync(userId.ToString())
                )))
                .Filter(req => req.auditUser != null, "Tài khoản đang thao tác xử lý đơn hàng không tồn tại hoặc bị xóa.")
                .Filter(req => req.order != null, $"Không tìm thấy đơn hàng phù hợp với mã đơn {orderCode}")
                .FilterAsync(async req => await userManager.IsInRoleAsync(req.auditUser, Constants.UserRoles.ADMIN), $"Đơn hàng {orderCode} không thể xử lý do người thao tác không phải là {Constants.UserRoles.ADMIN}.")
                .MapAsync(async req =>
                {
                    req.order.Status = (int)OrderStatusEnum.Delivery;
                    await this.orderRepository.UpdateAsync(req.order);
                    return req.order;
                });
        }

        public async Task<Option<Orders, string>> ChangeStatusToProcessOrderAsync(string orderCode, int userId)
        {
            return await (await (orderCode, userId)
                .SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => req.orderCode.IsNotNullOrEmpty(), "Mã của Đơn Tổng không hợp lệ.")
                .Filter(req => req.userId > 0, "Id của người dùng không hợp lệ.")
                .MapAsync(async req => (
                    order: await this.orderRepository.FirstOrDefaultAsync(new OrderFilterByOrderCodeSpecification(req.orderCode)),
                    auditUser: await userManager.FindByIdAsync(userId.ToString())
                )))
                .Filter(req => req.auditUser != null, "Tài khoản đang thao tác xử lý đơn hàng không tồn tại hoặc bị xóa.")
                .Filter(req => req.order != null, $"Không tìm thấy đơn hàng phù hợp với mã đơn {orderCode}")
                .FilterAsync(async req => await userManager.IsInRoleAsync(req.auditUser, Constants.UserRoles.ADMIN), $"Đơn hàng {orderCode} không thể xử lý do người thao tác không phải là {Constants.UserRoles.ADMIN}.")
                .MapAsync(async req =>
                {
                    req.order.Status = (int)OrderStatusEnum.Processing;
                    await this.orderRepository.UpdateAsync(req.order);
                    return req.order;
                });
        }

        public Task<Option<string, string>> CheckoutCartAsync(CheckoutCartViewModel data, EcommerceIdentityUser user)
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> CompleteOrderAsync(string orderCode, int userId, string note = "")
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> CompleteSummaryOrderAsync(int summaryOrderId)
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> CreateNormalOrderWithOptionAsync(CreateOrderWithOptionViewModel request)
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> CreateSaleOrderWithOptionAsync(CreateOrderWithOptionViewModel request)
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> CreateSummaryOrderAsync(CreateSummaryOrderViewModel inverseOrders, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> CreateWithdrawalOrderWithOptionAsync(CreateOrderWithOptionViewModel request)
        {
            throw new NotImplementedException();
        }

        public Task<ImmutableList<Orders>> GetAllOrdersByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Orders> GetOrderDetailByCodeAsync(string orderCode)
        {
            throw new NotImplementedException();
        }

        public Task<ImmutableList<Orders>> GetOrderDetailByParams(OrderStatusEnum? orderStatus, DateTime? fromDate, DateTime? toDate, string userPhone, string orderCode, ProductFilterParamViewModel[] productFilter, OrderTypeEnum? orderType, int? summaryOrderId)
        {
            throw new NotImplementedException();
        }

        public Task<Option<OrderExportExcelWrapper, string>> GetSummaryOrderDetailStyleAsExcelBytesAsync(int summaryOrderId)
        {
            throw new NotImplementedException();
        }

        public Task<Option<OrderExportExcelWrapper, string>> GetSummaryOrderPhoneCardWebsiteStyleAsExcelBytesAsync(int summaryOrderId)
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> CreateDepositOrderAsync(OrderViewModel request)
        {
            throw new NotImplementedException();
        }
    }
}