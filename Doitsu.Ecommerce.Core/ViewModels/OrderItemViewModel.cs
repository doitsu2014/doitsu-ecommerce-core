using AutoMapper;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core;
using Doitsu.Service.Core.Abstraction;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class OrderItemViewModel : BaseViewModel<OrderItems>
    {
        public OrderItemViewModel()
        {
        }

        public OrderItemViewModel(OrderItems entity, IMapper mapper) : base(entity, mapper)
        {
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int OrderId { get; set; }
        public decimal SubTotalPrice { get; set; }
        public int SubTotalQuantity { get; set; }
        public double? Discount { get; set; }
        public decimal SubTotalFinalPrice { get; set; }
        public string ProductName { get; set; }
    }
}