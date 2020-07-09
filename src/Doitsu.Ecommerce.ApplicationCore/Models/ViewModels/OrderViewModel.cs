using System;
using System.Collections.Generic;
using AutoMapper;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.ApplicationCore.Models.ViewModels
{
    public class ExportOrderToExcel
    {
        public static ExportOrderToExcel CreateInstance(byte[] content, string fileDownloadName)
        {
            return new ExportOrderToExcel
            {
                Content = content,
                FileDownloadName = fileDownloadName
            };
        }

        public byte[] Content { get; set; }
        public string FileDownloadName { get; set; }
        public string ContentType { get => Constants.FileExtension.EXCEL; }
    }
    public class CreateSummaryOrderViewModel
    {
        public string Note { get; set; }
        public List<OrderViewModel> Orders { get; set; }
    }
    public class OrderViewModel : BaseViewModel<Orders>
    {
        public OrderViewModel()
        {
        }

        public OrderViewModel(Orders entity, IMapper mapper) : base(entity, mapper)
        {
        }

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("discount")]
        public double Discount { get; set; }
        [JsonProperty("totalPrice")]
        public decimal TotalPrice { get; set; }
        [JsonProperty("finalPrice")]
        public decimal FinalPrice { get; set; }
        [JsonProperty("userId")]
        public int UserId { get; set; }
        [JsonProperty("totalQuantity")]
        public int TotalQuantity { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("deliveryAddress")]
        public string DeliveryAddress { get; set; }
        [JsonProperty("deliveryName")]
        public string DeliveryName { get; set; }
        [JsonProperty("deliveryPhone")]
        public string DeliveryPhone { get; set; }
        [JsonProperty("deliveryEmail")]
        public string DeliveryEmail { get; set; }

        [JsonProperty("deliveryCountry")]
        public string DeliveryCountry { get; set; }

        [JsonProperty("deliveryDistrict")]
        public string DeliveryDistrict { get; set; }

        [JsonProperty("deliveryCity")]
        public string DeliveryCity { get; set; }

        [JsonProperty("deliveryWard")]
        public string DeliveryWard { get; set; }

        [JsonProperty("deliverProvider")]
        public DeliverEnum? DeliveryProvider { get; set; }

        [JsonProperty("deliveryFees")]
        public decimal? DeliveryFees { get; set; }

        [JsonProperty("deliveryProviderReferenceLink")]
        public string DeliveryProviderReferenceLink { get; set; }

        [JsonProperty("deliveryProviderCode")]
        public string DeliveryProviderCode { get; set; }

        [JsonProperty("dynamic01")]
        public string Dynamic01 { get; set; }

        [JsonProperty("dynamic02")]
        public string Dynamic02 { get; set; }

        [JsonProperty("dynamic03")]
        public string Dynamic03 { get; set; }

        [JsonProperty("dynamic04")]
        public string Dynamic04 { get; set; }

        [JsonProperty("dynamic05")]
        public string Dynamic05 { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("cancelNote")]
        public string CancelNote { get; set; }

        [JsonProperty("priority")]
        public OrderPriorityEnum? Priority { get; set; }

        [JsonProperty("type")]
        public OrderTypeEnum Type { get; set; }

        [JsonProperty("paymentType")]
        public OrderPaymentTypeEnum? PaymentType { get; set; }

        [JsonProperty("paymentProofImageUrl")]
        public string PaymentProofImageUrl { get; set; }

        [JsonProperty("paymentValue")]
        public decimal PaymentValue { get; set; }
    }

    public class CreateOrderWithOptionViewModel : OrderViewModel
    {
        [JsonProperty("orderItems")]
        public ICollection<CreateOrderItemWithOptionViewModel> OrderItems { get; set; }
    }


    public class OrderDetailViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("discount")]
        public double Discount { get; set; }
        [JsonProperty("totalPrice")]
        public decimal TotalPrice { get; set; }
        [JsonProperty("finalPrice")]
        public decimal FinalPrice { get; set; }
        [JsonProperty("userId")]
        public int UserId { get; set; }
        [JsonProperty("totalQuantity")]
        public int TotalQuantity { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("deliveryAddress")]
        public string DeliveryAddress { get; set; }
        [JsonProperty("deliveryName")]
        public string DeliveryName { get; set; }
        [JsonProperty("deliveryPhone")]
        public string DeliveryPhone { get; set; }
        [JsonProperty("deliveryEmail")]
        public string DeliveryEmail { get; set; }
        [JsonProperty("deliveryCountry")]
        public string DeliveryCountry { get; set; }

        [JsonProperty("deliveryDistrict")]
        public string DeliveryDistrict { get; set; }

        [JsonProperty("deliveryCity")]
        public string DeliveryCity { get; set; }

        [JsonProperty("deliveryWard")]
        public string DeliveryWard { get; set; }

        [JsonProperty("deliveryFees")]
        public decimal? DeliveryFees { get; set; }

        [JsonProperty("deliverProvider")]
        public DeliverEnum? DeliveryProvider { get; set; }

        [JsonProperty("deliveryProviderReferenceLink")]
        public string DeliveryProviderReferenceLink { get; set; }

        [JsonProperty("deliveryProviderCode")]
        public string DeliveryProviderCode { get; set; }

        [JsonProperty("dynamic01")]
        public string Dynamic01 { get; set; }
        [JsonProperty("dynamic02")]
        public string Dynamic02 { get; set; }
        [JsonProperty("dynamic03")]
        public string Dynamic03 { get; set; }
        [JsonProperty("dynamic04")]
        public string Dynamic04 { get; set; }
        [JsonProperty("dynamic05")]
        public string Dynamic05 { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
        [JsonProperty("cancelNote")]
        public string CancelNote { get; set; }
        [JsonProperty("priority")]
        public OrderPriorityEnum? Priority { get; set; }
        [JsonProperty("type")]
        public OrderTypeEnum Type { get; set; }
        [JsonProperty("user")]
        public EcommerceIdentityUserViewModel User { get; set; }
        [JsonProperty("orderItems")]
        public virtual ICollection<OrderItemViewModel> OrderItems { get; set; }

        [JsonProperty("inverseSummaryOrders")]
        public virtual ICollection<OrderViewModel> InverseSummaryOrders { get; set; }

        [JsonProperty("paymentType")]
        public OrderPaymentTypeEnum? PaymentType { get; set; }
        [JsonProperty("paymentProofImageUrl")]
        public string PaymentProofImageUrl { get; set; }
        [JsonProperty("paymentValue")]
        public decimal PaymentValue { get; set; }
    }

    public class RequestChangeOrderNoteViewModel
    {
        public bool IsNormalNote { get; set; } = true;
        public string Note { get; set; }
    }

    public class RequestChangePaymentProof
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("paymentProofImageUrl")]
        public string PaymentProofImageUrl { get; set; }
    }

    public class RequestChangePaymentValue
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("paymentValue")]
        public decimal? PaymentValue { get; set; }
    }

    public class RequestChangeDeliveryProviderCode
    {
        [JsonProperty("deliveryProviderCode")]
        public string DeliveryProviderCode { get; set; }
    }

}