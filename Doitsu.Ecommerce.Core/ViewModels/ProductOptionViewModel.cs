using System.Collections.Generic;

namespace Doitsu.Ecommerce.Core.ViewModels
{

    public class BaseEditProductOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }
        public virtual ICollection<BaseEditProductOptionValueViewModel> ProductOptionValues { get; set; }
    }

    public class BaseEditProductOptionValueViewModel
    {
        public int Id { get; set; }
        public int ProductOptionId { get; set; }
        public string Value { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }
    }
}

