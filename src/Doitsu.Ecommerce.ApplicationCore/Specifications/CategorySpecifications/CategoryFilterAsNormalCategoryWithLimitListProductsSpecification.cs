using System.Linq;
using System;
using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;
using Doitsu.Ecommerce.ApplicationCore.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Specifications.CategorySpecifications
{
    public class CategoryFilterAsNormalCategoryWithLimitListProductsSpecification : BaseSpecification<Categories, Categories>
    {
        public CategoryFilterAsNormalCategoryWithLimitListProductsSpecification(string[] slugs, int limit = 0) : base(c => !c.IsFixed && c.ParentCateId != null && slugs.Contains(c.Slug))
        {
            AddInclude(c => c.ParentCate);
            AddInclude(c => c.Products);
            ApplySpecialSelector(limit);
        }

        public CategoryFilterAsNormalCategoryWithLimitListProductsSpecification(string parentCategorySlug, int limit = 0) : base(c => !c.IsFixed && c.ParentCate.Slug == parentCategorySlug)
        {
            AddInclude(c => c.ParentCate);
            AddInclude(c => c.Products);
            ApplySpecialSelector(limit);
        }

        private void ApplySpecialSelector(int limit = 0)
        {
            if (limit > 0)
            {
                Selector = c => new Categories()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Vers = c.Vers,
                    Active = c.Active,
                    ParentCateId = c.ParentCateId,
                    IsFixed = c.IsFixed,
                    Products = c.Products
                    .AsQueryable()
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.ProductVariantOptionValues)
                            .ThenInclude(pv => pv.ProductOptionValue)
                    .Include(p => p.ProductOptions)
                        .ThenInclude(po => po.ProductOptionValues)
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(limit)
                    .ToList()

                };
            }
            else
            {
                Selector = c => new Categories()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Vers = c.Vers,
                    Active = c.Active,
                    ParentCateId = c.ParentCateId,
                    IsFixed = c.IsFixed,
                    Products = c.Products
                        .AsQueryable()
                        .Include(p => p.ProductVariants)
                            .ThenInclude(p => p.ProductVariantOptionValues)
                        .Include(p => p.ProductOptions)
                            .ThenInclude(p => p.ProductOptionValues)
                                .ThenInclude(pov => pov.ProductVariantOptionValues)
                        .OrderByDescending(p => p.CreatedDate)
                        .ToList()
                };
            }
        }
    }
}