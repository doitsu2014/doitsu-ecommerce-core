using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using System.Linq;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ICategoryService : IBaseService<Categories>
    {
        Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryForMenuAsync();
        Task<ImmutableList<CategoryViewModel>> GetBuildingCategoryChildrenAsync();
        Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryChildrenAsync();

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
    }

    public class CategoryService : BaseService<Categories>, ICategoryService
    {
        public CategoryService(EcommerceDbContext dbContext,
                               IMapper mapper,
                               ILogger<BaseService<Categories, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
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

        public async Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryChildrenAsync() => (await this.Get(cate => cate.ParentCate.Slug == Constants.SuperFixedCategorySlug.PRODUCT)
                .Where(cate => cate.IsFixed)
                .Include(cate => cate.InverseParentCate)
                .ProjectTo<CategoryMenuViewModel>(Mapper.ConfigurationProvider)
                .ToListAsync())
                .ToImmutableList();

        public async Task<ImmutableList<CategoryWithInverseParentViewModel>> GetInverseCategory(string slug = default, int depth = default)
        {
            var query = this.Get(cate => cate.Slug == slug)
                .Include(cate => cate.InverseParentCate);
            for (var i = 0; i < depth; ++i)
            {
                query = query.ThenInclude(cate => cate.InverseParentCate);
            }

            var result = await query.ToListAsync();
            var listCategory = result.Select(c => Mapper.Map<CategoryWithInverseParentViewModel>(c)).ToList();

            return listCategory.ToImmutableList();
        }
    }
}
