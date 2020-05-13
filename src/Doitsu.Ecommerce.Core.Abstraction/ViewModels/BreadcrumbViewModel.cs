using System.Collections.Generic;

namespace Doitsu.Ecommerce.Core.Abstraction.ViewModels
{
    public class BreadcrumbItemViewModel
    {
        public string Title { get; set; }
        public string Link { get; set; }
    }

    public class BreadcrumbViewModel
    {
        public BreadcrumbViewModel()
        {
            BreadcrumbPreviousItems = new List<BreadcrumbItemViewModel>();
            Current = new BreadcrumbItemViewModel();
        }
        public BreadcrumbItemViewModel Current { get; set; }
        public ICollection<BreadcrumbItemViewModel> BreadcrumbPreviousItems { get; set; }
    }
}