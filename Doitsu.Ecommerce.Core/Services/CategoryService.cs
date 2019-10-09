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
    }

    public class CategoryService : BaseService<Categories>, ICategoryService
    {
        public CategoryService(IUnitOfWork unitOfWork, ILogger<BaseService<Categories>> logger) : base(unitOfWork, logger)
        {

        }

        public async Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryForMenuAsync()
        {
            var listFixedCategory =
                await this.Get(cate => cate.ParentCate.Slug == Constants.SuperFixedCategorySlug.PRODUCT && cate.IsFixed)
                .ProjectTo<CategoryMenuViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider)
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
                .ProjectTo<CategoryMenuViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cate => cate.Slug == parentCateSlug);
            var listChild = parentCategory.InverseParentCate.ToImmutableList();

            return listChild;
        }

        public async Task<ImmutableList<CategoryViewModel>> GetBuildingCategoryChildrenAsync()
        {
            var listFixedCategory = await this.Get(cate => cate.ParentCate.Slug == Constants.SuperFixedCategorySlug.BUILDING && cate.IsFixed)
                .ProjectTo<CategoryViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider)
                .ToListAsync();

            return listFixedCategory.ToImmutableList();
        }

        public async Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryChildrenAsync()
        {
            var listFixedCategory =
            await this.Get(cate => cate.ParentCate.Slug == Constants.SuperFixedCategorySlug.PRODUCT && cate.IsFixed)
                .Include(cate => cate.InverseParentCate)
                .ProjectTo<CategoryMenuViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider)
                .ToListAsync();

            return listFixedCategory.ToImmutableList();
        }
    }
}
