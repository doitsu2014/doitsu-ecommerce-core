using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Doitsu.Ecommerce.Core.SEO.Interfaces;
using Microsoft.Extensions.Options;
using Schema.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doitsu.Ecommerce.Core.SEO.Services
{
    public class BreadcrumbGenerator : IBreadcrumbGenerator
    {
        private string baseUri;
        public BreadcrumbGenerator(IOptions<DomainModel> domainModel)
        {
            this.baseUri = domainModel.Value.BaseUri;
        }
        public BreadcrumbList GenerateBreadcrumbFromProductDetail(CategoryWithParentViewModel category, Organization organization)
        {
            try
            {
                var breadcrumb = new BreadcrumbList
                {
                    ItemListElement = new List<IListItem>
                    {
                       new ListItem
                       {
                           Position = 1,
                           Item = new Thing
                           {
                               Id = organization.Url,
                               Name = organization.Name
                           }
                       },
                       new ListItem
                       {
                           Position = 2,
                           Item = new Thing
                           {
                               Id = new Uri(baseUri + "/" + category.Slug),
                               Name = category.Name
                           }
                       },
                    }
                };

                return breadcrumb;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GenerateBreadcrumbJsonLdFromProductDetail(CategoryWithParentViewModel category, Organization organization)
        {
            return GenerateBreadcrumbFromProductDetail(category, organization).ToString();
        }
    }
}
