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
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Optional;
using Optional.Async;
using Doitsu.Utils;

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

        Task<Option<int, string>> CreateProductWithOptionAsync(CreateProductViewModel data);
        Task<Option<int[], string>> CreateProductWithOptionAsync(ICollection<CreateProductViewModel> data);
        Task<Option<int, string>> UpdateProductWithOptionAsync(UpdateProductViewModel data);
        Task<Option<int, string>> UpdateProductVariantsAsync(ProductVariantViewModel data);
        Task<ProductVariantDetailViewModel> FindProductVariantFromOptionsAsync(ICollection<ProductOptionValueViewModel> listProductOptions);
    }

    public class ProductService : BaseService<Products>, IProductService
    {
        public ProductService(EcommerceDbContext dbContext,
                            IMapper mapper,
                            ILogger<BaseService<Products, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }

        private async Task<CategoryWithProductOverviewViewModel> GetFirstCategoryWithProductByCateSlug(string slug)
            => await DbContext.Set<Categories>().Where(c => c.Slug == slug).ProjectTo<CategoryWithProductOverviewViewModel>(Mapper.ConfigurationProvider).FirstOrDefaultAsync();

        public async Task<ImmutableList<ProductOverviewViewModel>> GetOverProductsByCateIdAsync(string cateSlug)
        {
            var category = await GetFirstCategoryWithProductByCateSlug(cateSlug);
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
                            pro => Mapper.Map<ProductOverviewViewModel>(pro)
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
                    .Get(x => x.Cate.Slug == buildingCategory.Slug)
                    .ProjectTo<ProductOverviewViewModel>(Mapper.ConfigurationProvider);

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
                await this.SelfRepository
                .AsNoTracking()
                .ProjectTo<ProductDetailViewModel>(Mapper.ConfigurationProvider)
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
                .Select(x => Mapper.Map<ProductOverviewViewModel>(x));

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

            var result = productList.Select(x => Mapper.Map<ProductOverviewViewModel>(x));
            return result.ToImmutableList();
        }

        private async Task<IQueryable<Products>> QueryAllOriginProductsInSuperParentCategoryAsync(string superParentCateSlug)
        {
            // query categories
            var allParentCategoriesOfProduct = (await GetRepository<Categories>()
                    .AsNoTracking()
                    .Where(x => x.ParentCate != null && x.ParentCate.Slug == superParentCateSlug)
                    .ProjectTo<CategoryMenuViewModel>(Mapper.ConfigurationProvider)
                    .ToListAsync()).ToImmutableList();

            var sortedSetInverseCategoryIds = new SortedSet<int>();
            foreach (var parentCategory in allParentCategoriesOfProduct)
            {
                var inverseCategoryIds = parentCategory.InverseParentCate.Select(ic => ic.Id).ToImmutableSortedSet();
                if (inverseCategoryIds.Count > 0)
                {
                    foreach (var inverseCategoryId in inverseCategoryIds)
                        sortedSetInverseCategoryIds.Add(inverseCategoryId);
                }
                else
                {
                    sortedSetInverseCategoryIds.Add(parentCategory.Id);
                }
            }

            // query products through categories ids
            var cateIds = sortedSetInverseCategoryIds.AsEnumerable();
            var productsQuery = this.Get(pro => pro.CateId.HasValue);
            productsQuery = productsQuery
                .Include(pro => pro.Cate)
                .Include(pro => pro.Cate.ParentCate)
                .Where(pro => cateIds.Contains(pro.CateId.Value));

            return productsQuery;
        }

        private string GetSkuAttributeString(string value, int length = 0)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var noWhiteSpace = value.Replace(" ", "_").ToUpper();

            if (noWhiteSpace.Length <= length || length == 0)
            {
                return noWhiteSpace;
            }
            else
            {
                noWhiteSpace = noWhiteSpace.Substring(0, length);
                return noWhiteSpace;
            }
        }

        private ImmutableList<ProductVariants> BuildListProductVariant(Products product)
        {
            if (product.ProductOptions == null || product.ProductOptions.Count == 0)
            {
                return ImmutableList<ProductVariants>.Empty;
            }

            var productVariants = product.ProductOptions
                .Select(po => po.ProductOptionValues)
                .CartesianProduct()
                // Mapping cartesian to product variant
                .Select(listProductOptionValues =>
                {
                    var productVariant = new ProductVariants()
                    {
                        ProductId = product.Id,
                        AnotherPrice = 0,
                        AnotherDiscount = 0,
                        InventoryQuantity = 0,
                        Sku = $"{product.Code}",
                        Product = product,
                        ProductVariantOptionValues = new List<ProductVariantOptionValues>()
                    };

                    foreach (var productOptionValue in listProductOptionValues)
                    {
                        var productOption = product.ProductOptions.FirstOrDefault(po => po.ProductOptionValues.Any(pov => pov.Value == productOptionValue.Value));
                        productVariant.Sku = string.Join('/', new string[] { productVariant.Sku, GetSkuAttributeString(productOptionValue.Value) });
                        productVariant.ProductVariantOptionValues.Add(new ProductVariantOptionValues()
                        {
                            ProductOptionId = productOption.Id,
                            ProductOptionValueId = productOptionValue.Id,
                            ProductOptionValue = productOptionValue,
                            ProductOption = productOption
                        });
                    }

                    return productVariant;
                })
                .Select(productVariant =>
                {
                    var listDbProductVariants = product.ProductVariants?.ToList() ?? new List<ProductVariants>();
                    var exist = listDbProductVariants.FirstOrDefault(dbPv =>
                    {
                        if (dbPv.ProductVariantOptionValues.Count != productVariant.ProductVariantOptionValues.Count)
                        {
                            return false;
                        }
                        var listDbPvovIds = dbPv.ProductVariantOptionValues.Select(dbPvov => dbPvov.ProductOptionValueId);
                        var listCurrentPvovIds = productVariant.ProductVariantOptionValues.Select(dbPvov => dbPvov.ProductOptionValueId);
                        if (listDbPvovIds.Except(listCurrentPvovIds).Count() > 0)
                        {
                            return false;
                        }
                        return true;
                    });

                    if (exist != null)
                    {
                        productVariant.Id = exist.Id;
                        productVariant.AnotherPrice = exist.AnotherPrice;
                        productVariant.AnotherDiscount = exist.AnotherDiscount;
                        productVariant.InventoryQuantity = exist.InventoryQuantity;

                        productVariant.ProductVariantOptionValues = productVariant.ProductVariantOptionValues.Select(pvov =>
                        {
                            pvov.Id = pvov.Id;
                            pvov.ProductVariantId = exist.Id;
                            return pvov;
                        }).ToImmutableList();
                    }

                    return productVariant;
                })
                .ToImmutableList();

            return productVariants;
        }

        public async Task<Option<int, string>> CreateProductWithOptionAsync(CreateProductViewModel data)
        {
            using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
            {
                return await data.SomeNotNull()
                    .WithException("Dữ liệu truyền vào bị rỗng")
                    .MapAsync(async d =>
                    {
                        var productEnt = this.Mapper.Map<Products>(d);
                        var listProductVariants = this.BuildListProductVariant(productEnt);
                        productEnt.ProductVariants = listProductVariants;

                        await CreateAsync(productEnt);
                        await DbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return productEnt.Id;
                    });
            }
        }

        public async Task<Option<int, string>> UpdateProductWithOptionAsync(UpdateProductViewModel data)
        {
            using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
            {
                return await data.SomeNotNull()
                    .WithException("Dữ liệu truyền vào bị rỗng")
                    .MapAsync(async d =>
                    {
                        var currentProduct = await this.GetAsNoTracking(prod => prod.Id == d.Id)
                            .Include(prod => prod.ProductTag)
                            .Include(prod => prod.ProductOptions)
                                .ThenInclude(po => po.ProductOptionValues)
                            .Include(prod => prod.ProductVariants)
                                .ThenInclude(pv => pv.ProductVariantOptionValues)
                                .FirstOrDefaultAsync();
                        this.Mapper.Map(d, currentProduct);

                        return currentProduct;
                    })
                    .MapAsync(async productEnt =>
                    {
                        var listGeneratedProductVariants = this.BuildListProductVariant(productEnt);
                        listGeneratedProductVariants.ForEach(gPv =>
                        {
                            var modified = productEnt.ProductVariants.FirstOrDefault(pv => pv.Id == gPv.Id && pv.Id > 0);
                            if (modified != null)
                            {
                                // is modified some fields 
                                modified.Sku = gPv.Sku;
                            }
                            else
                            {
                                // is add new
                                productEnt.ProductVariants.Add(gPv);
                            }
                        });

                        this.Update(productEnt);
                        await this.DbContext.SaveChangesAsync();

                        return (
                            ProductEntId: productEnt.Id,
                            ProductEntProductVariants: productEnt.ProductVariants,
                            ListPoUnavailable: productEnt
                                .ProductOptions
                                .SelectMany(po => po.ProductOptionValues)
                                .Where(pov => pov.Status == ProductOptionValueStatusEnum.Unavailable)
                                .Select(x => x.Id)
                                .Distinct()
                                .ToImmutableList()
                            );
                    })
                    .MapAsync(async d =>
                    {
                        // Make product variant have any unavailable product option value to unavailable
                        d.ProductEntProductVariants.ToList().ForEach(pva =>
                        {
                            var isUnavailable = pva.ProductVariantOptionValues.Any(pvov => d.ListPoUnavailable.Contains(pvov.ProductOptionValueId ?? int.MinValue));
                            if (isUnavailable)
                            {
                                pva.Status = ProductVariantStatusEnum.Unavailable;
                            }
                            else
                            {
                                pva.Status = ProductVariantStatusEnum.Available;
                            }
                            this.DbContext.Entry(pva).State = EntityState.Modified;
                        });

                        await this.DbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return d.ProductEntId;
                    });
            }
        }

        Task<Option<int, string>> IProductService.UpdateProductVariantsAsync(ProductVariantViewModel data)
        {
            throw new NotImplementedException();
        }

        public async Task<Option<int[], string>> CreateProductWithOptionAsync(ICollection<CreateProductViewModel> data)
        {
            using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
            {
                return await data.SomeNotNull()
                    .WithException("Dữ liệu truyền vào bị rỗng")
                    .MapAsync(async d =>
                    {
                        var result = new List<Products>();
                        foreach (var prodMapping in d)
                        {
                            var productEnt = this.Mapper.Map<Products>(prodMapping);
                            var listProductVariants = this.BuildListProductVariant(productEnt);
                            productEnt.ProductVariants = listProductVariants;
                            await CreateAsync(productEnt);
                            result.Add(productEnt);
                        }
                        
                        await DbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return result.Select(x => x.Id).ToArray();
                    });
            }
        }

        public async Task<ProductVariantDetailViewModel> FindProductVariantFromOptionsAsync(ICollection<ProductOptionValueViewModel> listProductOptions)
        {
            var poIds = listProductOptions.Select(po => po.Id);
            var totalIds = poIds.Count();

            return await this.DbContext.ProductVariants.AsNoTracking()
                .Where(pv => pv.ProductVariantOptionValues.Count() == totalIds && 
                    pv.ProductVariantOptionValues.Select(pvov => pvov.ProductOptionValueId ?? int.MinValue).All(pvovPovId => poIds.Contains(pvovPovId)))
                    .ProjectTo<ProductVariantDetailViewModel>(this.Mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
        }
    }
}