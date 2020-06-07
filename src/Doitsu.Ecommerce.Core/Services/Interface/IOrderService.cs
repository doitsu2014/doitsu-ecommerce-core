using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;

using Optional;


namespace Doitsu.Ecommerce.Core.Services.Interface
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

        Task<ImmutableList<OrderDetailViewModel>> GetAllOrdersByUserIdAsync(int userId);

        Task<OrderDetailViewModel> GetOrderDetailByCodeAsync(string orderCode);

        Task<ImmutableList<OrderDetailViewModel>> GetOrderDetailByParams(OrderStatusEnum? orderStatus,
            DateTime? fromDate,
            DateTime? toDate,
            string userPhone,
            string orderCode,
            ProductFilterParamViewModel[] productFilter,
            OrderTypeEnum? orderType,
            int? summaryOrderId);

        /// <summary>
        /// Create a Normal Order, just create a Order from option and do nothing with user's balance. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CreateNormalOrderWithOptionAsync(CreateOrderWithOptionViewModel request);

        /// <summary>
        /// Create a Slae Order.
        /// A order is called as a Sale Order, if a transaction would be associated to a user, who create that order, and then update that user's balance.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CreateSaleOrderWithOptionAsync(CreateOrderWithOptionViewModel request);

        /// <summary>
        /// Create Withdrawl order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CreateWithdrawalOrderWithOptionAsync(CreateOrderWithOptionViewModel request);

        /// <summary>
        /// Create Deposit order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CreateDepositOrderAsync(OrderViewModel request);

        /// <summary>
        /// This feature just use for Orders with New Status, and Sale Type
        /// Inverse Orders will be overrided to Processing Status if feature done.
        /// </summary>
        /// <param name="inverseOrders"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CreateSummaryOrderAsync(CreateSummaryOrderViewModel inverseOrders, int userId);

        /// <summary>
        /// Get bytes array of Export Summary Excel (detail style)
        /// </summary>
        /// <param name="summaryOrderId"></param>
        /// <returns></returns>
        Task<Option<ExportOrderToExcel, string>> GetSummaryOrderDetailStyleAsExcelBytesAsync(int summaryOrderId);

        /// <summary>
        /// Get bytes array of Export Summary Excel (phonecard website style)
        /// </summary>
        /// <param name="summaryOrderId"></param>
        /// <returns></returns>
        Task<Option<ExportOrderToExcel, string>> GetSummaryOrderPhoneCardWebsiteStyleAsExcelBytesAsync(int summaryOrderId);

        /// <summary>
        /// Complete the summary order and included different cancelled orders.
        /// </summary>
        /// <param name="summaryOrderId">Summary Order Id to Query</param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CompleteSummaryOrderAsync(int summaryOrderId);

        Task<Option<OrderViewModel, string>> CancelSummaryOrderAsync(int summaryOrderId, int auditUserId, string cancelNote = "");

        Task<Option<OrderViewModel, string>> CancelOrderAsync(string orderCode, int userId, string cancelNote = "");

        Task<Option<OrderViewModel, string>> CompleteOrderAsync(string orderCode, int userId, string note = "");

        /// <summary>
        /// Superpower function, to work correctly we have to complete
        /// TODO: Wrap all logic change order status
        /// CURRENT: just as a controller pass cancel or done status
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="statusEnum"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum, int auditUserId, string note = "", bool isWorkingInventoryQuantity = false);
        Task<Option<OrderViewModel, string>> ChangeOrderNote(int orderId, string note = "");
        Task<Option<OrderViewModel, string>> ChangeOrderCancelNote(int orderId, string note = "");
        Task<Option<OrderViewModel, string>> ChangeOrderPaymentProofImageUrlAsync(int orderId, string proof = "");
        Task<Option<OrderViewModel, string>> ChangeOrderPaymentValueAsync(int orderId, decimal? paymentValue);
        Task<Option<OrderViewModel, string>> ChangeStatusToDeliveryOrderAsync(string orderCode, int userId, bool isWorkingInventoryQuantity = false);
        Task<Option<OrderViewModel, string>> ChangeStatusToProcessOrderAsync(string orderCode, int userId);
        Task<Option<OrderViewModel, string>> ChangeOrderDeliveryProviderCodeAsync(int orderId, string code);
    }


}