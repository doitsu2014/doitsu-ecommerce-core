using System.Collections.Generic;

namespace Doitsu.Ecommerce.Core.ViewModels
{

    public class ProductOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }
        public virtual ICollection<ProductOptionValueViewModel> ProductOptionValues { get; set; }
    }

    public class ProductOptionValueViewModel
    {
        public int Id { get; set; }
        public int ProductOptionId { get; set; }
        public string Value { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }
        public ProductOptionValueStatusEnum Status { get; set; }
    }
}

