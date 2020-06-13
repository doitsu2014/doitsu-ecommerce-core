using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Schema.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doitsu.Ecommerce.Core.SEO.Interfaces
{
    public interface IWebsiteGenerator
    {
        WebSite GenerateWebsite();
        string GenerateWebsiteJsonLd();
        Organization GenerateOrganizationFromBrand(BrandViewModel brand);
        string GenerateOrganizationJsonLdFromBrand(BrandViewModel brand);
    }
}
