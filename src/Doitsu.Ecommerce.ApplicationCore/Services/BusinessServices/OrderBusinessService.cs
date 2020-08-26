using System;
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
        // private readonly IServiceScopeFactory serviceScopeFactory;
        // private readonly IDeliveryIntegrator deliveryIntegrator;
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

        public async Task<Option<Orders, string>> CancelOrderAsync(string orderCode, int userId, string cancelNote = "")
        {
            using (var transaction = await this.databaseManager.GetDatabaseContextTransactionAsync())
            {
                return await CancelOrderInternalAsync(orderCode, userId, cancelNote)
                    .MapAsync(async res =>
                    {
                        await transaction.CommitAsync();
                        return res;
                    });
            }
        }

        private async Task<Option<Orders, string>> CancelOrderInternalAsync(string orderCode, int auditUserId, string cancelNote = "")
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
                        return await this.UpdateUserBalanceAsync(order, ImmutableList<ProductVariants>.Empty, UserTransactionTypeEnum.Rollback)
                            .MapAsync(async d =>
                            {
                                await this.orderRepository.UpdateAsync(order);
                                return order;
                            });
                    }
                });
        }

        public async Task<Option<Orders, string>> CancelSummaryOrderAsync(int summaryOrderId, int auditUserId, string cancelNote = "")
        {
            using (var transaction = await this.databaseManager.GetDatabaseContextTransactionAsync())
            {
                return await(summaryOrderId, auditUserId, cancelNote).SomeNotNull()
                    .WithException("Mã của Đơn Tổng không hợp lệ.")
                    .MapAsync(async req =>
                    {
                        var summaryOrder = await this.GetAsTracking(o => o.Id == req && o.Type == OrderTypeEnum.Summary)
                            .FirstOrDefaultAsync();
                        summaryOrder.Status = (int)OrderStatusEnum.Cancel;
                        summaryOrder.CancelNote = $"{cancelNote}";
                        this.Update(summaryOrder);

                        var listUpdatingInverseOrderCode = (await this.GetAsNoTracking(o => o.SummaryOrderId == summaryOrder.Id && o.Type == OrderTypeEnum.Sale)
                            .Where(inverseOrder => inverseOrder.Status != (int)OrderStatusEnum.Done)
                            .Select(io => io.Code)
                            .ToListAsync())
                            .ToImmutableList();

                        foreach (var updatingOrderCode in listUpdatingInverseOrderCode)
                        {
                            await CancelOrderInternalAsync(updatingOrderCode, auditUserId, cancelNote);
                        }

                        await this.CommitAsync();
                        await transaction.CommitAsync();
                        return this.Mapper.Map<OrderViewModel>(summaryOrder);
                    });
            }

            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> ChangeOrderCancelNote(int orderId, string note = "")
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> ChangeOrderDeliveryProviderCodeAsync(int orderId, string code)
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> ChangeOrderNote(int orderId, string note = "")
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> ChangeOrderPaymentProofImageUrlAsync(int orderId, string proof = "")
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> ChangeOrderPaymentValueAsync(int orderId, decimal? paymentValue)
        {
            throw new NotImplementedException();
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
                        return await ChangeStatusToDeliveryOrderAsync(orderCode, d.auditUserId, d.isWorkingInventoryQuantity);
                    default:
                        return Option.None<Orders, string>("Không thể chuyển đơn hàng sang trạng thái này.");
                }
            });

        public Task<Option<Orders, string>> ChangeStatusToDeliveryOrderAsync(string orderCode, int userId, bool isWorkingInventoryQuantity = false)
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> ChangeStatusToProcessOrderAsync(string orderCode, int userId)
        {
            throw new NotImplementedException();
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

        public Task<Option<Orders, string>> CreateDepositOrderAsync(Orders request)
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

        public Task<Option<Orders, string>> CreateSummaryOrderAsync(CreateSummaryOrders inverseOrders, int userId)
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
    }
}