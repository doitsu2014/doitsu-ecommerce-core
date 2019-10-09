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