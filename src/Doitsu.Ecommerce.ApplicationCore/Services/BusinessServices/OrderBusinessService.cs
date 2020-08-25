using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Models.ExportModels;
using Doitsu.Ecommerce.ApplicationCore.Models.ViewModels;
using Optional;

namespace Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices
{
    public class OrderBusinessService : IOrderBusinessService
    {
        public Task<Option<Orders, string>> CancelOrderAsync(string orderCode, int userId, string cancelNote = "")
        {
            throw new NotImplementedException();
        }

        public Task<Option<Orders, string>> CancelSummaryOrderAsync(int summaryOrderId, int auditUserId, string cancelNote = "")
        {
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

        public Task<Option<Orders, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum, int auditUserId, string note = "", bool isWorkingInventoryQuantity = false)
        {
            throw new NotImplementedException();
        }

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

        public Task<Option<Orders, string>> CreateDepositOrderAsync(OrderViewModel request)
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
    }
}