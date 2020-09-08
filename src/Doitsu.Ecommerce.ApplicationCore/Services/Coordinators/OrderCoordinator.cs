using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.Coordinators;
using Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Async;
using Optional.Collections;

namespace Doitsu.Ecommerce.ApplicationCore.Services.Coordinators
{
    public class OrderCoordinator : IOrderCoordinator
    {
        private readonly ILogger<OrderCoordinator> logger;
        private readonly IBaseEcommerceRepository<Orders> orderRepository;
        private readonly IBaseEcommerceRepository<Products> productRepository;
        private readonly IBaseEcommerceRepository<ProductVariants> productVariantRepository;
        private readonly IBaseEcommerceRepository<UserTransaction> userTransactionRepository;
        private readonly IOrderBusinessService orderBusinessService;
        private readonly IProductBusinessService productBusinessService;
        private readonly IUserTransactionBusinessService userTransactionBusinessService;
        private readonly IEcommerceDatabaseManager databaseManager;

        public OrderCoordinator(ILogger<OrderCoordinator> logger,
                                IOrderBusinessService orderBusinessService,
                                IProductBusinessService productBusinessService,
                                IUserTransactionBusinessService userTransactionBusinessService,
                                IEcommerceDatabaseManager databaseManager)
        {
            this.logger = logger;
            this.orderBusinessService = orderBusinessService;
            this.productBusinessService = productBusinessService;
            this.userTransactionBusinessService = userTransactionBusinessService;
            this.databaseManager = databaseManager;
        }

        public async Task<Option<Orders, string>> CancelOrderAndUpdateUserBalanceAsync(string orderCode, int userId, string cancelNote = "")
        {
            using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
            {
                return await this.orderBusinessService.CancelOrderAsync(orderCode, userId, cancelNote)
                    .FlatMapAsync(async order => await this.userTransactionBusinessService
                        .UpdateUserBalanceAsync(order, ImmutableList<ProductVariants>.Empty, UserTransactionTypeEnum.Rollback)
                        .MapAsync(async d =>
                        {
                            await transaction.CommitAsync();
                            return order;
                        }));
            }
        }

        public async Task<Option<Orders, string>> CancelSummaryOrderAndUpdateUserBalanceAsync(int summaryOrderId, int auditUserId, string cancelNote = "")
        {
            using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
            {
                return await this.orderBusinessService.CancelSummaryOrderAsync(summaryOrderId, auditUserId, cancelNote)
                    .MapAsync(async o => await Task.FromResult((o, auditUserId, cancelNote)))
                    .FlatMapAsync(async req =>
                    {
                        var listResult = new List<Option<(int userId, UserTransaction userTransaction), string>>();
                        foreach (var saleOrder in req.o.InverseSummaryOrders)
                        {
                            listResult.Add(await this.userTransactionBusinessService.UpdateUserBalanceAsync(saleOrder, ImmutableList<ProductVariants>.Empty, UserTransactionTypeEnum.Rollback));
                        }
                        if (!listResult.All(r => r.HasValue))
                        {
                            return Option.None<Orders, string>(listResult.Exceptions().Aggregate((a, b) => $"{a},\n{b}"));
                        }
                        return Option.Some<Orders, string>(req.o);
                    })
                    .MapAsync(async d =>
                    {
                        await transaction.CommitAsync();
                        return d;
                    });
            }
        }

        public async Task<Option<Orders, string>> ChangeOrderStatus(string orderCode, OrderStatusEnum statusEnum, int auditUserId, string note = "", bool isWorkingInventoryQuantity = false)
        {
            using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
            {
                return await (orderCode, auditUserId, statusEnum, note, isWorkingInventoryQuantity).SomeNotNull()
                    .WithException(string.Empty)
                    .FilterAsync(async d => await this.orderRepository.AnyAsync(new OrderFilterByOrderCodeSpecification(d.orderCode)), $"Không thể tim thấy đơn hàng có mã #{orderCode} cần đổi trạng thái.")
                    .FlatMapAsync(async d =>
                    {
                        switch (statusEnum)
                        {
                            case OrderStatusEnum.Cancel:
                                return await this.orderBusinessService.CancelOrderAsync(d.orderCode, d.auditUserId, d.note);
                            case OrderStatusEnum.Done:
                                return await this.orderBusinessService.CompleteOrderAsync(d.orderCode, d.auditUserId, d.note);
                            case OrderStatusEnum.Processing:
                                return await this.orderBusinessService.ChangeStatusToProcessOrderAsync(d.orderCode, d.auditUserId);
                            case OrderStatusEnum.Delivery:
                                return await (await this.orderBusinessService.ChangeStatusToDeliveryOrderAsync(d.orderCode, d.auditUserId))
                                    .Map(result => (order: result, isWorkingInventoryQuantity))
                                    .FlatMapAsync(async req =>
                                    {
                                        if (req.isWorkingInventoryQuantity)
                                        {
                                            // TODO: Work with normal product
                                            // var orderedProductIds = orderItems.Where(oi => oi.ProductVariantId == null).Select(oi => oi.ProductId);
                                            var orderedPvIds = req.order.OrderItems
                                            .Where(oi => oi.ProductVariantId != null)
                                            .Select(oi => (oi.ProductVariantId.Value, oi.SubTotalQuantity))
                                            .ToArray();

                                            return await this.productBusinessService.DecreaseBatchPvInventoryQuantityAsync(orderedPvIds)
                                                .MapAsync(
                                                    async ids => await Task.FromResult(req.order)
                                                );
                                        }
                                        return Option.Some<Orders, string>(req.order);
                                    });
                            default:
                                return Option.None<Orders, string>("Không thể chuyển đơn hàng sang trạng thái này.");
                        }
                    })
                    .MapAsync(async result =>
                    {
                        await transaction.CommitAsync();
                        return result;
                    });
            }
        }
    }
}