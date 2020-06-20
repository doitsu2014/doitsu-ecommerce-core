using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using AutoMapper.QueryableExtensions;
using Optional;
using Optional.Async;
using Doitsu.Utils;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IProductVariantService : IBaseService<ProductVariants>
    {
        Task<ProductVariantDetailViewModel> FindProductVariantFromOptionsAsync(ICollection<ProductOptionValueViewModel> listProductOptions);
        Task<ImmutableArray<int?>> GetProductVariantIdsFromProductFilterParamsAsync(ProductFilterParamViewModel[] productFilterParams);
        Task<Option<int, string>> UpdateProductVariantsAsync(ProductVariantViewModel data);
        Task<Option<int, string>> UpdateProductVariantAnotherDiscountAsync(int productId, int productVariantId, float anotherDiscount);
        Task<Option<int, string>> UpdateProductVariantAnotherPriceAsync(int productId, int productVariantId, decimal anotherPrice);
        Task<Option<int, string>> UpdateProductVariantStatusAsync(int productId, int productVariantId, ProductVariantStatusEnum value);
        ImmutableList<ProductVariants> BuildListProductVariant(Products product);
        Task<Option<int, string>> IncreaseInventoryQuantityAsync(int productId, int productVariantId, int quantity = 0);
        Task<Option<int, string>> DecreaseInventoryQuantityAsync(int productId, int productVariantId, int quantity = 0);
        Task<Option<int[], string>> IncreaseBatchPvInventoryQuantityAsync(int[] productVariantIds, int quantity = 0);
        Task<Option<int[], string>> DecreaseBatchPvInventoryQuantityAsync(int[] productVariantIds, int quantity = 0);
    }

    public class ProductVariantService : BaseService<ProductVariants>, IProductVariantService
    {
        public ProductVariantService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<ProductVariants, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }

        public async Task<ImmutableArray<int?>> GetProductVariantIdsFromProductFilterParamsAsync(ProductFilterParamViewModel[] productFilterParams)
        {
            var productIds = productFilterParams.Select(pf => pf.Id);
            var listProductFilterParams = productFilterParams.AsEnumerable();
            var listCondition = new List<Expression<Func<ProductVariants, bool>>>();
            foreach (var item in productFilterParams)
            {
                var selectedValueIds = item.ProductOptions.Select(po => po.SelectedValueId).ToArray();
                var countSelectedValue = selectedValueIds.Count();
                listCondition.Add((pv =>
                    pv.ProductId == item.Id &&
                    (
                        countSelectedValue == 0 || pv.ProductVariantOptionValues.Select(pvov => pvov.ProductOptionValueId).All(pvovId => selectedValueIds.Contains(pvovId))
                    )));
            }
            var productVariants = this.DbContext
                    .ProductVariants
                    .AsNoTracking();
            foreach (var condition in listCondition)
            {
                productVariants = productVariants.Where(condition);
            }
            return (await productVariants.Select(pv => (int?)pv.Id).ToListAsync()).ToImmutableArray();
        }

        public async Task<ProductVariantDetailViewModel> FindProductVariantFromOptionsAsync(ICollection<ProductOptionValueViewModel> listProductOptions)
        {
            var poIds = listProductOptions.Where(po => po != null).Select(po => po.Id);
            var totalIds = poIds.Count();

            return await this.DbContext.ProductVariants.AsNoTracking()
                .Where(pv => pv.ProductVariantOptionValues.Count() == totalIds &&
                    pv.ProductVariantOptionValues.Select(pvov => pvov.ProductOptionValueId ?? int.MinValue).All(pvovPovId => poIds.Contains(pvovPovId)))
                .ProjectTo<ProductVariantDetailViewModel>(this.Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public Task<Option<int, string>> UpdateProductVariantsAsync(ProductVariantViewModel data)
        {
            throw new NotImplementedException();
        }

        public async Task<Option<int, string>> UpdateProductVariantAnotherDiscountAsync(int productId, int productVariantId, float anotherDiscount)
        {
            return await (productId, productVariantId, anotherDiscount).SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async req => await DbContext.Products.AnyAsync(p => p.Id == productId), "Không tồn tại sản phẩm này.")
                .FilterAsync(async req => await DbContext.ProductVariants.AnyAsync(pv => pv.Id == productVariantId), "Không tồn tại biến thể này.")
                .MapAsync(async req =>
                {
                    var productVariant = await this.FindByKeysAsync(productVariantId);
                    productVariant.AnotherDiscount = anotherDiscount;
                    this.Update(productVariant);
                    await this.CommitAsync();
                    return productVariant.Id;
                });
        }

        public async Task<Option<int, string>> UpdateProductVariantAnotherPriceAsync(int productId, int productVariantId, decimal anotherPrice)
        {
            return await (productId, productVariantId, anotherPrice).SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async req => await DbContext.Products.AnyAsync(p => p.Id == productId), "Không tồn tại sản phẩm này.")
                .FilterAsync(async req => await DbContext.ProductVariants.AnyAsync(pv => pv.Id == productVariantId), "Không tồn tại biến thể này.")
                .MapAsync(async req =>
                {
                    var productVariant = await this.FindByKeysAsync(productVariantId);
                    productVariant.AnotherPrice = anotherPrice;
                    this.Update(productVariant);
                    await this.CommitAsync();
                    return productVariant.Id;
                });
        }

        public async Task<Option<int, string>> UpdateProductVariantStatusAsync(int productId, int productVariantId, ProductVariantStatusEnum value)
        {
            return await (productId, productVariantId, value).SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async req => await DbContext.Products.AnyAsync(p => p.Id == productId), "Không tồn tại sản phẩm này.")
                .FilterAsync(async req => await DbContext.ProductVariants.AnyAsync(pv => pv.Id == productVariantId), "Không tồn tại biến thể này.")
                .MapAsync(async req =>
                {
                    var productVariant = await this.FindByKeysAsync(productVariantId);
                    productVariant.Status = value;
                    this.Update(productVariant);
                    await this.CommitAsync();
                    return productVariant.Id;
                });
        }

        public ImmutableList<ProductVariants> BuildListProductVariant(Products product)
        {

            Func<ProductOptions, bool> predicatePoExistAndActive = po => po.Id == 0 || (po.Id != 0 && po.Active);
            if (product.ProductOptions == null || product.ProductOptions.Where(predicatePoExistAndActive).Count() == 0)
            {
                return ImmutableList<ProductVariants>.Empty;
            }

            var productVariants = product.ProductOptions
                .Where(predicatePoExistAndActive)
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

        public async Task<Option<int, string>> IncreaseInventoryQuantityAsync(int productId, int productVariantId, int quantity = 0)
        {
            return await (productId, productVariantId, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể thêm số lượng sản phẩm trong kho vì số lượng bạn nhập {quantity} nhỏ hơn 0.")
                .MapAsync(async req => (product: await this.DbContext.Products.FindAsync(req.productId), productVariant: await this.FindByKeysAsync(productVariantId), quantity))
                .FilterAsync(async req => await Task.FromResult(req.product != null), "Không tồn tại sản phẩm theo id đang thao tác.")
                .FilterAsync(async req => await Task.FromResult(req.productVariant != null), "Không tồn tại biến thể theo id đang thao tác.")
                .MapAsync(async req =>
                {
                    req.productVariant.InventoryQuantity += quantity;
                    req.product.InventoryQuantity += quantity;
                    this.Update(req.productVariant);
                    this.DbContext.Products.Update(req.product);
                    await this.CommitAsync();
                    return req.productVariant.Id;
                });
        }

        public async Task<Option<int, string>> DecreaseInventoryQuantityAsync(int productId, int productVariantId, int quantity = 0)
        {
            return await (productId, productVariantId, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể giảm số lượng sản phẩm trong kho vì số lượng truyền vào {quantity} nhỏ hơn 0.")
                .MapAsync(async req => (product: await this.DbContext.Products.FindAsync(req.productId), productVariant: await this.FindByKeysAsync(productVariantId), quantity))
                .FilterAsync(async req => await Task.FromResult(req.product != null), "Không tồn tại sản phẩm theo id đang thao tác.")
                .FilterAsync(async req => await Task.FromResult(req.productVariant != null), "Không tồn tại biến thể theo id đang thao tác.")
                .FlatMapAsync(async req =>
                {
                    if (req.productVariant.InventoryQuantity < quantity || req.product.InventoryQuantity < quantity)
                    {
                        return Option.None<int, string>("Sản phẩm và biến thể hiện tại không còn đủ số lượng để xuất kho.");
                    }
                    else
                    {
                        req.productVariant.InventoryQuantity -= quantity;
                        this.Update(req.productVariant);
                        req.product.InventoryQuantity -= quantity;
                        this.DbContext.Products.Update(req.product);
                        await this.CommitAsync();
                        return Option.Some<int, string>(req.productVariant.Id);
                    }
                });
        }

        public async Task<Option<int[], string>> IncreaseBatchPvInventoryQuantityAsync(int[] productVariantIds, int quantity = 0)
        {
            return await (productVariantIds, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể thêm số lượng sản phẩm trong kho vì số lượng bạn nhập {quantity} nhỏ hơn 0.")
                .MapAsync(async req =>
                {
                    var listPv = await this.Get(pv => req.productVariantIds.Contains(pv.Id), pv => pv.Include(qPv => qPv.Product)).ToListAsync();
                    listPv.ForEach(pv =>
                    {
                        pv.InventoryQuantity += quantity;
                        this.Update(pv);

                        pv.Product.InventoryQuantity += (listPv.Count() * quantity);
                        this.DbContext.Products.Update(pv.Product);
                    });

                    await this.CommitAsync();
                    return req.productVariantIds;
                });
        }

        public async Task<Option<int[], string>> DecreaseBatchPvInventoryQuantityAsync(int[] productVariantIds, int quantity = 0)
        {
            return await (productVariantIds, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể giảm số lượng sản phẩm trong kho vì số lượng truyền vào {quantity} nhỏ hơn 0.")
                .FlatMapAsync(async req =>
                {
                    var listPv = await this.Get(pv => req.productVariantIds.Contains(pv.Id),
                        pv => pv.Include(qPv => qPv.Product))
                        .ToListAsync();
                    var listUnavailablePv = listPv.Where(pv => pv.InventoryQuantity < quantity).ToImmutableList();

                    if (listUnavailablePv.Count() > 0)
                    {
                        return Option.None<int[], string>(listUnavailablePv.Select(pv => $"Biến thể có SKU là {pv.Sku} hiện tại đã hết hàng.").Aggregate((a, b) => $"{a}\n{b}"));
                    }

                    var listAvailablePv = listPv.Where(pv => pv.InventoryQuantity >= quantity).ToImmutableList();
                    listAvailablePv.ForEach(pv =>
                    {
                        pv.InventoryQuantity -= quantity;
                        this.Update(pv);
                        pv.Product.InventoryQuantity -= (listAvailablePv.Count() * quantity);
                        this.DbContext.Products.Update(pv.Product);
                    });

                    await this.CommitAsync();
                    return Option.Some<int[], string>(req.productVariantIds);
                });
        }

    }
}
