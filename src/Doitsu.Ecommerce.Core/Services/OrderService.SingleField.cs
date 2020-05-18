using System;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.Core.Services
{
    public partial class OrderService
    {
        public async Task<Option<OrderViewModel, string>> ChangeOrderNote(int orderId, string note = "")
        {
            return await (orderId, note)
                .SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async data => await this.AnyAsync(o => o.Id == data.orderId), "Không tìm thấy đơn hàng cần thay đổi")
                .MapAsync(async data =>
                {
                    var order = await this.FindByKeysAsync(orderId);
                    order.Note = note;
                    this.Update(order);
                    await this.CommitAsync();
                    return this.Mapper.Map<OrderViewModel>(order);
                });
        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderCancelNote(int orderId, string note = "")
        {
            return await (orderId, note)
                .SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async data => await this.AnyAsync(o => o.Id == data.orderId), "Không tìm thấy đơn hàng cần thay đổi")
                .MapAsync(async data =>
                {
                    var order = await this.FindByKeysAsync(orderId);
                    order.CancelNote = note;
                    this.Update(order);
                    await this.CommitAsync();
                    return this.Mapper.Map<OrderViewModel>(order);
                });
        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderPaymentProofImageUrlAsync(int orderId, string proof = "")
        {
            return await (orderId, proof)
                .SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async data => await this.AnyAsync(o => o.Id == data.orderId), "Không tìm thấy đơn hàng cần thay đổi")
                .MapAsync(async data =>
                {
                    var order = await this.FindByKeysAsync(orderId);
                    order.PaymentProofImageUrl = proof;
                    this.Update(order);
                    await this.CommitAsync();
                    return this.Mapper.Map<OrderViewModel>(order);
                });
        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderPaymentValueAsync(int orderId, decimal? paymentValue)
        {
            return await (orderId, paymentValue)
                            .SomeNotNull()
                            .WithException(string.Empty)
                            .Filter(data => data.paymentValue != null, "Phải nhập giá trị thanh toán")
                            .FilterAsync(async data => await this.AnyAsync(o => o.Id == data.orderId), "Không tìm thấy đơn hàng cần thay đổi")
                            .MapAsync(async data =>
                            {
                                var order = await this.FindByKeysAsync(orderId);
                                order.PaymentValue = paymentValue.Value;
                                this.Update(order);
                                await this.CommitAsync();
                                return this.Mapper.Map<OrderViewModel>(order);
                            });
        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderDeliveryProviderCodeAsync(int orderId, string code)
        {
            return await (orderId, code)
                            .SomeNotNull()
                            .WithException(string.Empty)
                            .Filter(data => !data.code.IsNullOrEmpty(), "Phải nhập mã vận chuyển")
                            .FilterAsync(async data => await this.AnyAsync(o => o.Id == data.orderId), "Không tìm thấy đơn hàng cần thay đổi")
                            .MapAsync(async data =>
                            {
                                var order = await this.FindByKeysAsync(orderId);
                                order.DeliveryProviderCode = data.code;
                                this.Update(order);
                                await this.CommitAsync();
                                return this.Mapper.Map<OrderViewModel>(order);
                            });
        }
    }
}