using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;

namespace Doitsu.Ecommerce.Core.Services
{

    public interface IProductService : IBaseService<Products>
    {
        Task<ImmutableList<ProductOverviewViewModel>> GetOverProductsByCateIdAsync(string cateSlug);
        Task<ProductDetailViewModel> GetProductDetailBySlugAsync(string productSlug);
        Task<ImmutableList<ProductOverviewViewModel>> GetRandomProductAsync(int take);
        Task<ImmutableList<OverviewBuildingProductsViewModel>> GetOverviewBuildingProductsAsync(ImmutableList<CategoryViewModel> buildingCategories);

        /// <summary>
        /// Current context, the SuperParentCateogry include san-pham and cong-trinh
        /// So there are the static categories.
        /// We just query on this but do not edit.
        /// </summary>
        /// <param name="superParentCateSlug"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        Task<ImmutableList<ProductOverviewViewModel>> GetProductsFromSuperParentCateId(string superParentCateSlug, string productName = "", string productCode = "");
    }

    public class ProductService : BaseService<Products>, IProductService
    {
        public ProductService(IUnitOfWork unitOfWork, ILogger<BaseService<Products>> logger) : base(unitOfWork, logger)
        {

        }

        public async Task<ImmutableList<ProductOverviewViewModel>> GetOverProductsByCateIdAsync(string cateSlug)
        {
            var categorySerivce = this.UnitOfWork.GetService<ICategoryService>();
            var category = await categorySerivce
                .FirstOrDefaultActiveAsync<CategoryWithProductOverviewViewModel>(cate => cateSlug == cate.Slug);

            if (category == null)
            {
                return (new List<ProductOverviewViewModel>()).ToImmutableList();
            }

            var productsQuery =
                await QueryAllOriginProductsInSuperParentCategoryAsync(Constants.SuperFixedCategorySlug.PRODUCT);

            if (category.InverseParentCate.Count() > 0)
            {
                var listProducts = ImmutableList<ProductOverviewViewModel>.Empty;
                foreach (var childCate in category.InverseParentCate)
                {
                    var innerProducts = (await productsQuery
                            .Where(pro => pro.CateId == childCate.Id).ToListAsync())
                        .Select(
                            pro => this.UnitOfWork.Mapper.Map<ProductOverviewViewModel>(pro)
                        );

                    listProducts = listProducts.AddRange(innerProducts);
                }
                return listProducts;
            }
            else
            {
                var listProducts = category
                    .Products
                    .ToImmutableList();
                return listProducts;
            }
        }

        public async Task<ImmutableList<OverviewBuildingProductsViewModel>> GetOverviewBuildingProductsAsync(ImmutableList<CategoryViewModel> buildingCategories)
        {
            var result = ImmutableList<OverviewBuildingProductsViewModel>.Empty;
            foreach (var buildingCategory in buildingCategories)
            {
                var queryBuildings = this
                    .GetActive(x => x.Cate.Slug == buildingCategory.Slug)
                    .ProjectTo<ProductOverviewViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider);

                var count = await queryBuildings.CountAsync();
                if (count != 0)
                {
                    var overViewBuilding = OverviewBuildingProductsViewModel.Empty;
                    overViewBuilding.CategoryVm = buildingCategory;
                    var listBuildings = await queryBuildings.ToListAsync();
                    overViewBuilding.ProductOverviews = overViewBuilding.ProductOverviews.AddRange(listBuildings.ToImmutableList());
                    result = result.Add(overViewBuilding);
                }
            }
            return result;
        }

        public async Task<ProductDetailViewModel> GetProductDetailBySlugAsync(string productSlug)
        {
            var productDetailVM =
                await this.SelfDbSet
                .AsNoTracking()
                .ProjectTo<ProductDetailViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Slug == productSlug);

            return productDetailVM;
        }

        public async Task<ImmutableList<ProductOverviewViewModel>> GetRandomProductAsync(int take)
        {
            var rand = new Random();
            var productsQuery = await this.QueryAllOriginProductsInSuperParentCategoryAsync(Constants.SuperFixedCategorySlug.PRODUCT);

            var listShuffleOverview =
                (await productsQuery
                    .OrderByDescending(x => Guid.NewGuid())
                    .Skip(0)
                    .Take(take)
                    .ToListAsync())
                .Select(x => this.UnitOfWork.Mapper.Map<ProductOverviewViewModel>(x));

            return listShuffleOverview.ToImmutableList();
        }

        public async Task<ImmutableList<ProductOverviewViewModel>> GetProductsFromSuperParentCateId(string superParentCateSlug, string productName, string productCode)
        {
            var productsQuery = await QueryAllOriginProductsInSuperParentCategoryAsync(superParentCateSlug);
                
            if (!productName.IsNullOrEmpty())
            {
                var productNameTrim = productName.Trim();
                productsQuery = productsQuery.Where(pro => pro.Name.Contains(productNameTrim));
            }

            if (!productCode.IsNullOrEmpty())
            {
                var productCodeTrim = productCode.Trim();
                productsQuery = productsQuery.Where(pro => pro.Code.Contains(productCodeTrim));
            }

            var productList = await productsQuery
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var result = productList.Select(x => this.UnitOfWork.Mapper.Map<ProductOverviewViewModel>(x));
            return result.ToImmutableList();
        }

        private async Task<IQueryable<Products>> QueryAllOriginProductsInSuperParentCategoryAsync(string superParentCateSlug)
        {
            // query categories
            var categoryService = this.UnitOfWork.GetService<ICategoryService>();

            var allParentCategoriesOfProduct = (await categoryService
                .GetActive(x => x.ParentCate != null && x.ParentCate.Slug == superParentCateSlug)
                .ProjectTo<CategoryMenuViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider)
                .ToListAsync()).ToImmutableList();

            var sortedSetInverseCategoryIds = new SortedSet<int>();
            foreach (var parentCategory in allParentCategoriesOfProduct)
            {
                var inverseCategoryIds = parentCategory.InverseParentCate.Select(ic => ic.Id).ToImmutableSortedSet();
                foreach (var inverseCategoryId in inverseCategoryIds)
                    sortedSetInverseCategoryIds.Add(inverseCategoryId);
            }

            // query products through categories ids
            var cateIds = sortedSetInverseCategoryIds.AsEnumerable();
            var productsQuery = this.GetActive(pro => pro.CateId.HasValue);
            productsQuery = productsQuery
                .Include(pro => pro.Cate)
                .Include(pro => pro.Cate.ParentCate)
                .Where(pro => cateIds.Contains(pro.CateId.Value));

            return productsQuery;
        }
    }
}