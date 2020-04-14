using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core.Services.EmailService;
using Microsoft.Extensions.Logging;
using Optional;

namespace Doitsu.Ecommerce.Core.Services
{
    public partial class OrderService
    {
        #region Utilization 
        private string GetSkusAsString(IList<OrderItems> listOi)
        {
            if (listOi == null || listOi.Count == 0) return "";
            return listOi.Select(oi => oi.ProductVariant.Sku).Aggregate((x, y) => $"{x}\n{y}");
        }

        public string GetNormalizedOfType(OrderTypeEnum typeEnum)
        {
            return typeEnum
            switch
            {
                OrderTypeEnum.Summary => "Hóa đơn tổng",
                OrderTypeEnum.Sale => "Hóa đơn hàng bán",
                OrderTypeEnum.Desposit => "Hóa đơn nạp tiền",
                _ => "Hóa đơn rút tiền"
            };
        }

        private string GetNormalizedOfStatus(OrderStatusEnum statusEnum)
        {
            return statusEnum
            switch
            {
                OrderStatusEnum.New => "Đang chờ nạp",
                OrderStatusEnum.Processing => "Đang xử lý",
                OrderStatusEnum.Cancel => "Đã hủy",
                OrderStatusEnum.Done => "Hoàn thành",
                _ => "Thất bại"
            };
        }

        private string GetNormalizedDynamicDescriptionOrder(Orders order)
        {
            if (order == null) return string.Empty;
            else
            {
                var messages = new List<string>();
                if (!order.Dynamic01.IsNullOrEmpty()) messages.Add($"{order.Dynamic01}");
                if (!order.Dynamic02.IsNullOrEmpty()) messages.Add($"{order.Dynamic02}");
                if (!order.Dynamic03.IsNullOrEmpty()) messages.Add($"{order.Dynamic03}");
                if (!order.Dynamic04.IsNullOrEmpty()) messages.Add($"{order.Dynamic04}");
                if (!order.Dynamic05.IsNullOrEmpty()) messages.Add($"{order.Dynamic05}");
                return messages.Count > 0 ? messages.Aggregate((x, y) => $"{x}, {y}") : string.Empty;
            }
        }

        private string GetNormalizedOptionValueDescriptionOrder(OrderItems item)
        {
            if (item.ProductVariant?.ProductVariantOptionValues != null
                    && item.ProductVariant?.ProductVariantOptionValues?.Count > 0)
            {
                var optionValues = new List<string>();
                foreach (var ov in item.ProductVariant.ProductVariantOptionValues)
                {
                    optionValues.Add($"{ov.ProductOptionValue.ProductOption.Name}: {ov.ProductOptionValue.Value}");
                }
                return optionValues.Count > 0 ? optionValues.Aggregate((x, y) => $"{x}\n {y}") : string.Empty;
            }
            return string.Empty;
        }

        private async Task SendEmailToUsersAsync(Orders order)
        {
            try
            {
                var user = await userManager.FindByIdAsync(order.UserId.ToString());
                var messagePayloads = new List<MessagePayload>()
                {
                    await emailService.PrepareLeaderOrderMailConfirmAsync(order.User, order),
                    await emailService.PrepareCustomerOrderMailConfirm(order.User, order)
                };

                var emailResult = await emailService.SendEmailWithBachMocWrapperAsync(messagePayloads);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Process email for order {order.Code} failure.");
            }
        }
        #endregion
    }
}