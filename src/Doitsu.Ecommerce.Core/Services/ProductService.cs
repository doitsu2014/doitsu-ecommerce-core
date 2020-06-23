using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Doitsu.Ecommerce.Core.Abstraction;

using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Optional;
using Optional.Async;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{

    public interface IProductService : IEcommerceBaseService<Products>
    {
        Task<ImmutableList<ProductOverviewViewModel>> GetOverProductsByCateIdAsync(string cateSlug, int limit = 0);

        Task<ImmutableList<ProductOverviewViewModel>> GetOverProductsWithIQGreaterThanZeroAsync(string cateSlug);

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

        Task<Option<int, string>> CreateProductOptionAsync(int productId, ProductOptionViewModel data);

        Task<Option<int, string>> UpdateProductOptionAsync(int productId, int productOptionId, ProductOptionViewModel data);

        Task<Option<int, string>> DeleteProductOptionByKeyAsync(int productId, int productOptionId);

        Task<Option<int, string>> DecreaseInventoryQuantity(int productId, int quantity = 0);

        Task<Option<int, string>> IncreaseInventoryQuantity(int productId, int quantity = 0);
    }

    public class ProductService : EcommerceBaseService<Products>, IProductService
    {
        private readonly IProductVariantService productVariantService;
        private readonly IProductOptionService productOptionService;
        public ProductService(EcommerceDbContext dbContext,
            IMapper mapper,
            ILogger<EcommerceBaseService<Products>> logger,
            IProductVariantService productVariantService,
            IProductOptionService productOptionService) : base(dbContext, mapper, logger)
        {
            this.productVariantService = productVariantService;
            this.productOptionService = productOptionService;
        }

        public async Task<ImmutableList<ProductOverviewViewModel>> GetOverProductsByCateIdAsync(string cateSlug, int limit = 0)
        {
            var categories = DbContext.Set<Categories>()
                .Include(c => c.InverseParentCate)
                    .ThenInclude(c => c.Products)
                .Include(c => c.Products)
                .Where(c => c.Slug == cateSlug);
                
            var productsOfInverseCategories = categories
                .Where(c => c.InverseParentCate.Count() > 0)
                .SelectMany(c => c.InverseParentCate.SelectMany(cip => cip.Products))
                .OrderByDescending(p => p.CreatedDate)
                .AsQueryable();

            var products = categories
                .Select(c => c.Products)
                .SelectMany(listP => listP)
                .OrderByDescending(p => p.CreatedDate)
                .AsQueryable();

            if(limit > 0) 
            {
                productsOfInverseCategories = productsOfInverseCategories.Skip(0).Take(limit);
                products = products.Skip(0).Take(limit);
            } 

            return ImmutableList<ProductOverviewViewModel>
                .Empty
                .AddRange(await products.ProjectTo<ProductOverviewViewModel>(this.Mapper.ConfigurationProvider).ToListAsync())
                .AddRange(await productsOfInverseCategories.ProjectTo<ProductOverviewViewModel>(this.Mapper.ConfigurationProvider).ToListAsync()); 
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
                .ProjectTo<ProductOverviewViewModel>(this.Mapper.ConfigurationProvider)
                .ToListAsync();

            return productList.ToImmutableList();
        }

        private async Task<IQueryable<Products>> QueryAllOriginProductsInSuperParentCategoryAsync(string superParentCateSlug)
        {
            // query categories
            var allParentCategoriesOfProduct = (await GetRepository<Categories>()
                .AsNoTracking()
                .Include(c => c.InverseParentCate)
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
            var productsQuery = this.Get(
                    pro => pro.CateId.HasValue && cateIds.Contains(pro.CateId.Value),
                    pro => pro.Include(hPro => hPro.Cate).ThenInclude(hC => hC.ParentCate)
                );

            return productsQuery;
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
                        var listProductVariants = productVariantService.BuildListProductVariant(productEnt);
                        productEnt.ProductVariants = listProductVariants;

                        await CreateAsync(productEnt);
                        await CommitAsync();
                        await transaction.CommitAsync();
                        return productEnt.Id;
                    });
            }
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
                            var listProductVariants = productVariantService.BuildListProductVariant(productEnt);
                            productEnt.ProductVariants = listProductVariants;
                            await CreateAsync(productEnt);
                            result.Add(productEnt);
                        }

                        await CommitAsync();
                        await transaction.CommitAsync();
                        return result.Select(x => x.Id).ToArray();
                    });
            }
        }

        public async Task<Option<int, string>> CreateProductOptionAsync(int productId, ProductOptionViewModel data)
        {
            return await (productId, data).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => req.data != null, "Dữ liệu thuộc tính rỗng.")
                .FlatMapAsync(async req =>
                {
                    req.data.Name = req.data.Name.Trim();
                    var updateProductViewModel = await FirstOrDefaultAsync<UpdateProductViewModel>(prod => prod.Id == productId);
                    if (updateProductViewModel == null) return Option.None<int, string>($"Không tìm thấy sản phẩm tương ứng với id {req.productId}");
                    else if ((!await DbContext.ProductOptions.AnyAsync(po => po.Name == req.data.Name)))
                        updateProductViewModel.ProductOptions.Add(req.data);
                    updateProductViewModel.Name = req.data.Name;
                    return await UpdateProductWithOptionAsync(updateProductViewModel);
                });
        }

        public async Task<Option<int, string>> UpdateProductOptionAsync(int productId, int productOptionId, ProductOptionViewModel data)
        {
            return await (productId, productOptionId, data).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => req.data != null, "Dữ liệu thuộc tính rỗng.")
                .FlatMapAsync(async req =>
                {
                    var updateProductViewModel = await FirstOrDefaultAsync<UpdateProductViewModel>(prod => prod.Id == productId);
                    if (updateProductViewModel == null) return Option.None<int, string>($"Không tìm thấy sản phẩm tương ứng với id {req.productId}");
                    else if (!updateProductViewModel.ProductOptions.Any(po => po.Id == req.productOptionId)) return Option.None<int, string>($"Không tìm thấy mã thuộc tính tương ứng với id {req.productOptionId}");
                    else
                    {
                        var createNewValues = req.data.ProductOptionValues
                            .Where(reqDataPov => reqDataPov.Id == 0)
                            .Select(reqDataPov => reqDataPov.Value.Trim());
                        if (updateProductViewModel.ProductOptions.Any(po => po.Id == req.productOptionId &&
                                po.ProductOptionValues.Any(pov => createNewValues.Contains(pov.Value))))
                            return Option.None<int, string>($"Giá trị cho thuộc tính mà bạn muốn tạo mới đã tồn tại");
                    }

                    var updatePo = updateProductViewModel.ProductOptions.First(po => po.Id == req.productOptionId);
                    updatePo.ProductOptionValues = req.data.ProductOptionValues.Select(pov =>
                    {
                        pov.Value = pov.Value.Trim();
                        var existPov = updatePo.ProductOptionValues.FirstOrDefault(dbPov => dbPov.Id == pov.Id);
                        if (existPov != null)
                        {
                            pov.Vers = existPov.Vers;
                            pov.Active = existPov.Active;
                            pov.ProductOptionId = existPov.ProductOptionId;
                        }
                        return pov;
                    }).ToImmutableList();
                    updatePo.Name = data.Name.Trim();
                    return await UpdateProductWithOptionAsync(updateProductViewModel);
                });
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
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

                        // Change something
                        this.Mapper.Map(d, currentProduct);
                        await this.productOptionService.CreateAsync(currentProduct.ProductOptions.ToList().Where(po => po.Id == 0));
                        this.productOptionService.UpdateRange(currentProduct.ProductOptions.ToList().Where(po => po.Id != 0).ToList());
                        this.DbContext.Entry(currentProduct).State = EntityState.Modified;
                        await this.CommitAsync();
                        return currentProduct;
                    })
                    .MapAsync(async productEnt =>
                    {
                        var listGeneratedProductVariants = productVariantService.BuildListProductVariant(productEnt);
                        listGeneratedProductVariants.ForEach(gPv =>
                        {
                            if (productEnt.ProductVariants.Any(pv => pv.Id == gPv.Id && pv.Id > 0))
                            {
                                // modified variants
                                productEnt.ProductVariants.First(pv => pv.Id == gPv.Id && pv.Id > 0).Sku = gPv.Sku;
                            }
                            else
                            {
                                // Add new variant
                                productEnt.ProductVariants.Add(gPv);
                            }
                        });
                        await this.CommitAsync();
                        return await Task.FromResult((
                            ProductEntId: productEnt.Id,
                            ProductEntProductVariants: productEnt.ProductVariants,
                            ListPoUnavailable: productEnt.ProductOptions
                            .SelectMany(po => po.ProductOptionValues)
                            .Where(pov => pov.Status == ProductOptionValueStatusEnum.Unavailable)
                            .Select(x => x.Id)
                            .Distinct()
                            .ToImmutableList()
                        ));
                    })
                    .MapAsync(async d =>
                    {
                        // Make product variant have any unavailable product option value to unavailable
                        foreach (var pva in d.ProductEntProductVariants)
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
                            this.productVariantService.MarkModifiedOrAdded(pva);
                        }
                        await this.CommitAsync();
                        await transaction.CommitAsync();
                        return d.ProductEntId;
                    });
            }
        }

        public async Task<Option<int, string>> DeleteProductOptionByKeyAsync(int productId, int productOptionId)
        {
            using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
            {
                return await (productId, productOptionId)
                    .SomeNotNull()
                    .WithException("Id rỗng.")
                    .MapAsync(async req =>
                    {
                        var po = await this.DbContext.ProductOptions.Where(dPo => dPo.Id == req.productOptionId)
                            .Include(dPo => dPo.ProductOptionValues)
                            .ThenInclude(dPo => dPo.ProductVariantOptionValues)
                            .ThenInclude(dPo => dPo.ProductVariant)
                            .FirstOrDefaultAsync();
                        DbContext.ProductOptions.Remove(po);
                        DbContext.ProductOptionValues.RemoveRange(po.ProductOptionValues);
                        DbContext.ProductVariantOptionValues.RemoveRange(po.ProductOptionValues.SelectMany(pov => pov.ProductVariantOptionValues));
                        DbContext.ProductVariants.RemoveRange(po.ProductOptionValues.SelectMany(pov => pov.ProductVariantOptionValues.Select(pvov => pvov.ProductVariant)));
                        await this.DbContext.SaveChangesWithoutBeforeSavingAsync();
                        return req;
                    })
                    .MapAsync(async req =>
                    {
                        var productEnt = await this.DbContext.Products.Where(p => p.Id == req.productId)
                            .Include(prod => prod.ProductOptions)
                            .ThenInclude(po => po.ProductOptionValues)
                            .Include(p => p.ProductVariants)
                            .ThenInclude(p => p.ProductVariantOptionValues)
                            .FirstOrDefaultAsync();

                        var listGeneratedProductVariants = productVariantService.BuildListProductVariant(productEnt);
                        foreach (var gPv in listGeneratedProductVariants.Where(gPv => gPv.Id == 0).ToImmutableList())
                        {
                            await this.productVariantService.CreateAsync(gPv);
                        };
                        await this.DbContext.SaveChangesWithoutBeforeSavingAsync();
                        await transaction.CommitAsync();
                        return req.productOptionId;
                    });
            }
        }

        public async Task<Option<int, string>> IncreaseInventoryQuantity(int productId, int quantity = 0)
        {
            return await (productId, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể thêm số lượng sản phẩm trong kho vì số lượng bạn nhập {quantity} nhỏ hơn 0.")
                .MapAsync(async req => (product: await this.DbContext.Products.FindAsync(req.productId), quantity))
                .FilterAsync(async req => await Task.FromResult(req.product != null), "Không tồn tại sản phẩm theo id đang thao tác.")
                .MapAsync(async req =>
                {
                    req.product.InventoryQuantity += quantity;
                    this.Update(req.product);
                    await this.CommitAsync();
                    return req.product.Id;
                });
        }

        public async Task<Option<int, string>> DecreaseInventoryQuantity(int productId, int quantity = 0)
        {
            return await (productId, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể giảm số lượng sản phẩm trong kho vì số lượng truyền vào {quantity} nhỏ hơn 0.")
                .MapAsync(async req => (product: await this.DbContext.Products.FindAsync(req.productId), quantity))
                .FilterAsync(async req => await Task.FromResult(req.product != null), "Không tồn tại sản phẩm theo id đang thao tác.")
                .FlatMapAsync(async req =>
                {
                    if (req.product.InventoryQuantity < quantity)
                    {
                        return Option.None<int, string>("Sản phẩm hiện tại không còn đủ số lượng để xuất kho.");
                    }
                    else
                    {
                        req.product.InventoryQuantity -= quantity;
                        this.Update(req.product);
                        await this.CommitAsync();
                        return Option.Some<int, string>(req.product.Id);
                    }
                });
        }

        public async Task<ImmutableList<ProductOverviewViewModel>> GetOverProductsWithIQGreaterThanZeroAsync(string cateSlug)
        {
            return (await this.Get<ProductOverviewViewModel>(p => p.InventoryQuantity > 0
                    && p.ProductVariants.Where(pv => pv.InventoryQuantity > 0).Count() > 0)
                .ToListAsync()).ToImmutableList();
        }
    }
}