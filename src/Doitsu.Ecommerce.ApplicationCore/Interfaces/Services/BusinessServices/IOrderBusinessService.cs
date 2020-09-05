using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Models.ExportModels;
using Doitsu.Ecommerce.ApplicationCore.Models.ViewModels;
using Optional;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices
{
    public interface IOrderBusinessService
    {
        Task<ImmutableList<Orders>> GetAllOrdersByUserIdAsync(int userId);
        Task<Orders> GetOrderDetailByCodeAsync(string orderCode);
        Task<ImmutableList<Orders>> GetOrderDetailByParams(OrderStatusEnum? orderStatus,
            DateTime? fromDate,
            DateTime? toDate,
            string userPhone,
            string orderCode,
            ProductFilterParamViewModel[] productFilter,
            OrderTypeEnum? orderType,
            int? summaryOrderId);

        Task<Option<string, string>> CheckoutCartAsync(CheckoutCartViewModel data, EcommerceIdentityUser user);
        Task<Option<Orders, string>> CreateNormalOrderWithOptionAsync(CreateOrderWithOptionViewModel request);
        Task<Option<Orders, string>> CreateSaleOrderWithOptionAsync(CreateOrderWithOptionViewModel request);
        Task<Option<Orders, string>> CreateWithdrawalOrderWithOptionAsync(CreateOrderWithOptionViewModel request);
        Task<Option<Orders, string>> CreateDepositOrderAsync(OrderViewModel request);
        Task<Option<Orders, string>> CreateSummaryOrderAsync(CreateSummaryOrderViewModel inverseOrders, int userId);

        Task<Option<OrderExportExcelWrapper, string>> GetSummaryOrderDetailStyleAsExcelBytesAsync(int summaryOrderId);
        Task<Option<OrderExportExcelWrapper, string>> GetSummaryOrderPhoneCardWebsiteStyleAsExcelBytesAsync(int summaryOrderId);

        Task<Option<Orders, string>> CompleteSummaryOrderAsync(int summaryOrderId);
        Task<Option<Orders, string>> CancelSummaryOrderAsync(int summaryOrderId, int auditUserId, string cancelNote = "");
        Task<Option<Orders, string>> CancelOrderAsync(string orderCode, int userId, string cancelNote = "");
        Task<Option<Orders, string>> CompleteOrderAsync(string orderCode, int userId, string note = "");
        Task<Option<Orders, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum, int auditUserId, string note = "", bool isWorkingInventoryQuantity = false);
        Task<Option<Orders, string>> ChangeOrderNote(int orderId, string note = "");
        Task<Option<Orders, string>> ChangeOrderCancelNote(int orderId, string note = "");
        Task<Option<Orders, string>> ChangeOrderPaymentProofImageUrlAsync(int orderId, string proof = "");
        Task<Option<Orders, string>> ChangeOrderPaymentValueAsync(int orderId, decimal? paymentValue);
        Task<Option<Orders, string>> ChangeStatusToDeliveryOrderAsync(string orderCode, int userId);
        Task<Option<Orders, string>> ChangeStatusToProcessOrderAsync(string orderCode, int userId);
        Task<Option<Orders, string>> ChangeOrderDeliveryProviderCodeAsync(int orderId, string code); 
    }
}