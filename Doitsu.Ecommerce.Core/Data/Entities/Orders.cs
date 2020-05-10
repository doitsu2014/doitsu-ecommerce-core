using System;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;
using Doitsu.Ecommerce.Core.DeliveryIntegration;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class Orders : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Code { get; set; }
        public double Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FinalPrice { get; set; }

        public int UserId { get; set; }
        public int TotalQuantity { get; set; }
        public int Status { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }

        public int? RefernceDeliveryInformationId { get; set; }

        public string DeliveryAddress { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryPhone { get; set; }
        public string DeliveryEmail { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryDistrict { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryWard { get; set; }
        public DeliverEnum? DeliveryProvider { get; set; }
        public decimal? DeliveryFees { get; set; }

        public string Dynamic01 { get; set; }
        public string Dynamic02 { get; set; }
        public string Dynamic03 { get; set; }
        public string Dynamic04 { get; set; }
        public string Dynamic05 { get; set; }
        public string Note { get; set; }
        public string CancelNote { get; set; }
        public int? SummaryOrderId { get; set; }

        public OrderPaymentTypeEnum? PaymentType { get; set; }
        public string PaymentProofImageUrl { get; set; }

        public OrderTypeEnum Type { get; set; }
        public OrderPriorityEnum? Priority { get; set; }
        public byte[] Vers { get; set; }

        public virtual EcommerceIdentityUser User { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
        public virtual ICollection<UserTransaction> UserTransactions { get; set; }
        public virtual ICollection<Orders> InverseSummaryOrders { get; set; }
        public virtual Orders SummaryOrder { get; set; }
        public virtual DeliveryInformation RefernceDeliveryInformation { get; set; }
    }
}
