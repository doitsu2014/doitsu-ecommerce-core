using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Schema.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doitsu.Ecommerce.Core.SEO.Interfaces
{
    public interface IBreadcrumbGenerator
    {
        BreadcrumbList GenerateBreadcrumbFromProductDetail(CategoryWithParentViewModel category, Organization organization);
        string GenerateBreadcrumbJsonLdFromProductDetail(CategoryWithParentViewModel category, Organization organization);
    }
}
