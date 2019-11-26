using System;
using System.Collections.Generic;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core;
using Doitsu.Service.Core.Abstraction;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class OrderViewModel : BaseViewModel<Orders>
    {
        public OrderViewModel()
        {
        }

        public OrderViewModel(Orders entity, IMapper mapper) : base(entity, mapper)
        {
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public double Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public int UserId { get; set; }
        public int TotalQuantity { get; set; }
        public int Status { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryPhone { get; set; }
        public string DeliveryEmail { get; set; }
        public string Dynamic01 { get; set; }
        public string Dynamic02 { get; set; }
        public string Dynamic03 { get; set; }
        public string Dynamic04 { get; set; }
        public string Dynamic05 { get; set; }
    }

     public class CreateOrderWithOptionViewModel : OrderViewModel
     {
         public ICollection<ProductOptionValueViewModel> ProductOptionValues {get;set;}
     }


    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public int TotalQuantity { get; set; }
        public int Status { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public EcommerceIdentityUserViewModel User { get; set; }
        public ICollection<OrderItemViewModel> OrderItems { get; set; }
    }
}