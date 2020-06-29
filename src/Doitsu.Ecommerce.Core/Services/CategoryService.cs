using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;
using Doitsu.Ecommerce.Core.Extensions;
using Doitsu.Ecommerce.Core.Services.Interface;
using Microsoft.EntityFrameworkCore.Internal;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ICategoryService : IEcommerceBaseService<Categories>
    {
        Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryForMenuAsync();

        Task<ImmutableList<CategoryViewModel>> GetBuildingCategoryChildrenAsync();

        Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryChildrenAsync(string superFixedCategorySlug = Constants.SuperFixedCategorySlug.PRODUCT);

        /// <summary>
        /// Return empty list if @parentCateSlug is null or empty
        /// </summary>
        /// <param name="parentCateSlug"></param>
        /// <returns></returns>
        Task<ImmutableList<CategoryViewModel>> GetCategoryForParentCateSlugAsync(string parentCateSlug);

        /// <summary>
        /// Get category with inverse parent category
        /// </summary>
        /// <param name="slug">The identity value to find the category</param>
        /// <param name="depth">The depth of list including inverse parent category</param>
        /// <returns></returns>
        Task<ImmutableList<CategoryWithInverseParentViewModel>> GetInverseCategory(string slug = Constants.SuperFixedCategorySlug.PRODUCT, int depth = 1);

        Task<ImmutableList<CategoryWithInverseParentViewModel>> GetAllParentCategoryWithInverseCategory(int depth = 1);

        Task<ImmutableList<CategoryWithProductOverviewViewModel>> GetAllCategoriesWithProductAsync(string parentSlug = "", int limit = 0);
    }

    public class CategoryService : EcommerceBaseService<Categories>, ICategoryService
    {
        public CategoryService(EcommerceDbContext dbContext,
                               IMapper mapper,
                               ILogger<EcommerceBaseService<Categories>> logger) : base(dbContext, mapper, logger)
        {
        }

        public async Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryForMenuAsync()
        {
            var listFixedCategory =
                await this.Get(cate => cate.ParentCateId != null && cate.ParentCate.Slug == Constants.SuperFixedCategorySlug.PRODUCT && cate.IsFixed)
                .ProjectTo<CategoryMenuViewModel>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return listFixedCategory.ToImmutableList();
        }


        public async Task<ImmutableList<CategoryViewModel>> GetCategoryForParentCateSlugAsync(string parentCateSlug)
        {
            if (parentCateSlug.IsNullOrEmpty())
            {
                return (new List<CategoryViewModel>()).ToImmutableList();
            }

            var parentCategory = await this.GetAll()
                .ProjectTo<CategoryMenuViewModel>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cate => cate.Slug == parentCateSlug);
            var listChild = parentCategory.InverseParentCate.ToImmutableList();

            return listChild;
        }

        public async Task<ImmutableList<CategoryViewModel>> GetBuildingCategoryChildrenAsync() => (await this.Get(cate => cate.ParentCate.Slug == Constants.SuperFixedCategorySlug.BUILDING)
                .Where(cate => cate.IsFixed)
                .ProjectTo<CategoryViewModel>(Mapper.ConfigurationProvider)
                .ToListAsync()).ToImmutableList();

        public async Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryChildrenAsync(string supertFixedCategorySlug = Constants.SuperFixedCategorySlug.PRODUCT) => (await this.Get(cate => cate.ParentCate.Slug == supertFixedCategorySlug)
                .Where(cate => cate.IsFixed)
                .Include(cate => cate.InverseParentCate)
                .ProjectTo<CategoryMenuViewModel>(Mapper.ConfigurationProvider)
                .ToListAsync())
                .ToImmutableList();

        public async Task<ImmutableList<CategoryWithInverseParentViewModel>> GetInverseCategory(string slug = default, int depth = default)
        {
            var query = this.Get(cate => cate.Slug == slug)
                            .IncludeByDepth(cate => cate.InverseParentCate, depth);

            var result = await query.ToListAsync();
            var listCategory = result.Select(c => Mapper.Map<CategoryWithInverseParentViewModel>(c)).ToList();

            return listCategory.ToImmutableList();
        }

        public async Task<ImmutableList<CategoryWithInverseParentViewModel>> GetAllParentCategoryWithInverseCategory(int depth = 1)
        {
            var query = this.Get(cate => cate.ParentCateId == null && cate.IsFixed)
                            .IncludeByDepth(cate => cate.InverseParentCate, depth);

            var result = await query.ToListAsync();
            var listCategory = result.Select(c => Mapper.Map<CategoryWithInverseParentViewModel>(c)).ToList();

            return listCategory.ToImmutableList();
        }

        public async Task<ImmutableList<CategoryWithProductOverviewViewModel>> GetAllCategoriesWithProductAsync(string parentSlug, int limit = 0)
        {
            if (parentSlug.IsNullOrEmpty()) return ImmutableList<CategoryWithProductOverviewViewModel>.Empty;
            var query = this.Get(c => c.Slug == parentSlug)
                // Filter certainly parent category
                .Where(c => c.InverseParentCate.Count > 0)
                .Include(c => c.InverseParentCate)
                    .ThenInclude(ipc => ipc.Products)
                        .ThenInclude(p => p.Cate)
                .SelectMany(c => c.InverseParentCate)
                .Select(c =>
                    new Categories
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Slug = c.Slug,
                        Vers = c.Vers,
                        Active = c.Active,
                        ParentCateId = c.ParentCateId,
                        IsFixed = c.IsFixed,
                        Products = limit > 0
                            ? c.Products
                                .AsQueryable()
                                .Include(p => p.ProductVariants)
                                    .ThenInclude(pv => pv.ProductVariantOptionValues)
                                        .ThenInclude(pv => pv.ProductOptionValue)
                                .Include(p => p.ProductOptions)
                                    .ThenInclude(po => po.ProductOptionValues)
                                .OrderByDescending(p => p.CreatedDate)
                                .Take(limit)
                                .ToList()

                            : c.Products
                                .AsQueryable()
                                .Include(p => p.ProductVariants)
                                    .ThenInclude(p => p.ProductVariantOptionValues)
                                .Include(p => p.ProductOptions)
                                    .ThenInclude(p => p.ProductOptionValues)
                                        .ThenInclude(pov => pov.ProductVariantOptionValues)
                                .OrderByDescending(p => p.CreatedDate)
                                .ToList()
                    }
                );

            var result = (await query
                .ToListAsync())
                .Select(c => this.Mapper.Map<CategoryWithProductOverviewViewModel>(c));

            return result.ToImmutableList();
        }
    }
}