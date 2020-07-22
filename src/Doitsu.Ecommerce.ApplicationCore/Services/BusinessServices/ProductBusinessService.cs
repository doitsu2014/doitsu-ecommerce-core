using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Specifications.ProductVariantSpecifications;
using Doitsu.Ecommerce.ApplicationCore.Utils;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices
{
    public class ProductBusinessService : IProductService
    {
        private readonly IBaseEcommerceRepository<Products> productRepository;
        private readonly IBaseEcommerceRepository<ProductVariants> productVariantRepository;
        private readonly IEcommerceDatabaseManager databaseManager;
        private readonly ILogger<ProductBusinessService> logger;

        public ProductBusinessService(IBaseEcommerceRepository<Products> productRepository,
                                      IBaseEcommerceRepository<ProductVariants> productVariantRepository,
                                      IEcommerceDatabaseManager databaseManager,
                                      ILogger<ProductBusinessService> logger)
        {
            this.databaseManager = databaseManager;
            this.logger = logger;
            this.productVariantRepository = productVariantRepository;
            this.productRepository = productRepository;
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

        /// <summary>
        /// Best function ever 
        /// Build and redefine a list of product variants from list product options and existed list product variants
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
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

        public async Task<Option<int[], string>> DecreaseBatchPvInventoryQuantityAsync((int productVariantId, int quantity)[] listProductIdAndQuantity)
        {
            return await (listProductIdAndQuantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => listProductIdAndQuantity.All(x => x.quantity >= 0), $"Không thể giảm số lượng sản phẩm trong kho vì số lượng truyền vào nhỏ hơn 0.")
                .FlatMapAsync(async req =>
                {
                    using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
                    {
                        var productVariantIds = req.Select(x => x.productVariantId).ToArray();
                        var aggregateQuantity = req.Select(x => x.quantity).Aggregate((a, b) => a + b);
                        var listPvAndQuantity = (await productVariantRepository.ListAsync(new ProductVariantFilterByIdsSpecification(productVariantIds)))
                            .Select(pv => (req.FirstOrDefault(x => x.productVariantId == pv.Id).quantity, pv))
                            .ToArray();

                        // check if does exist any unavailable product variant, return None
                        var listUnavailablePv = listPvAndQuantity.Where(x => x.pv.InventoryQuantity < x.quantity);
                        if (listUnavailablePv.Count() > 0) return Option.None<int[], string>(listUnavailablePv.Select(x => $"Biến thể có SKU là {x.pv.Sku} hiện tại đã hết hàng.").Aggregate((a, b) => $"{a}\n{b}"));

                        // prepare list available product variants to update
                        var listAvailablePv = listPvAndQuantity
                            .Where(x => x.pv.InventoryQuantity >= x.quantity)
                            .ToList();

                        var updatingListAvailPv = listAvailablePv.Select(x =>
                            {
                                x.pv.InventoryQuantity -= x.quantity;
                                return x.pv;
                            })
                            .ToArray();
                        await productVariantRepository.UpdateRangeAsync(updatingListAvailPv);

                        var listProductIdAndQuantity = listAvailablePv
                            .Select(x => (x.pv.ProductId, x.quantity))
                            .GroupBy(y => y.ProductId)
                            .Select(y => new
                            {
                                Key = y.Key,
                                TotalQuantity = y.Select(iY => iY.quantity).Aggregate((a, b) => a + b)
                            })
                            .ToArray();

                        var listUpdatedProducts = (await productRepository.ListAsync(new ProductFilterByIdsSpecification(listProductIdAndQuantity.Select(l => l.Key).ToArray())))
                            .Select(p =>
                            {
                                p.InventoryQuantity -= listProductIdAndQuantity.First(piaq => piaq.Key == p.Id).TotalQuantity;
                                return p;
                            })
                            .ToArray();
                        await productRepository.UpdateRangeAsync(listUpdatedProducts);

                        await transaction.CommitAsync();
                        return Option.Some<int[], string>(productVariantIds);
                    }
                });
        }

        public async Task<Option<int, string>> DecreaseInventoryQuantityAsync(int productId, int productVariantId, int quantity = 0)
        {
            return await(productId, productVariantId, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể giảm số lượng sản phẩm trong kho vì số lượng truyền vào {quantity} nhỏ hơn 0.")
                .MapAsync(async req => (product: await productRepository.FindByKeysAsync(req.productId), productVariant: await productVariantRepository.FindByKeysAsync(productVariantId), quantity))
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
                        using (var transaction = await this.databaseManager.GetDatabaseContextTransactionAsync())
                        {
                            req.productVariant.InventoryQuantity -= quantity;
                            await productVariantRepository.UpdateAsync(req.productVariant);
                            req.product.InventoryQuantity -= quantity;
                            await productRepository.UpdateAsync(req.product);
                            await transaction.CommitAsync();
                            return Option.Some<int, string>(req.productVariant.Id);
                        }
                    }
                });
        }

        public async Task<Option<int[], string>> IncreaseBatchPvInventoryQuantityAsync((int productVariantId, int quantity)[] listProductIdAndQuantity)
        {
            return await (listProductIdAndQuantity)
                .SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => listProductIdAndQuantity.All(x => x.quantity >= 0), $"Không thể thêm số lượng sản phẩm trong kho vì số lượng bạn nhập nhỏ hơn 0.")
                .MapAsync(async req =>
                {
                    using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
                    {
                        var productVariantIds = req.Select(x => x.productVariantId).ToArray();
                        var listAvailablePv = (await this.productVariantRepository.ListAsync(new ProductVariantFilterByIdsSpecification(productVariantIds)))
                            .Select(pv => (pv, req.FirstOrDefault(x => x.productVariantId == pv.Id).quantity))
                            .ToImmutableArray();
                        var updatingListPv = listAvailablePv
                            .Select(x =>
                            {
                                x.pv.InventoryQuantity += x.quantity;
                                return x.pv;
                            })
                            .ToArray();
                        await productVariantRepository.UpdateRangeAsync(updatingListPv);

                        var listProductIdAndQuantity = listAvailablePv
                            .Select(x => (x.pv.ProductId, x.quantity))
                            .GroupBy(y => y.ProductId)
                            .Select(y => new
                            {
                                Key = y.Key,
                                TotalQuantity = y.Select(iY => iY.quantity).Aggregate((a, b) => a + b)
                            })
                            .ToArray();

                        var listUpdatedProducts = (await productRepository.ListAsync(new ProductFilterByIdsSpecification(listProductIdAndQuantity.Select(y => y.Key).ToArray())))
                            .Select(p =>
                            {
                                p.InventoryQuantity += listProductIdAndQuantity.First(piaq => piaq.Key == p.Id).TotalQuantity;
                                return p;
                            })
                            .ToArray();
                        await productRepository.UpdateRangeAsync(listUpdatedProducts);

                        await transaction.CommitAsync();
                        return productVariantIds;
                    }
                });
        }

        public async Task<Option<int, string>> IncreaseInventoryQuantityAsync(int productId, int productVariantId, int quantity = 0)
        {
            this.logger.LogDebug($"[{nameof(ProductBusinessService)}].[{nameof(IncreaseInventoryQuantityAsync)}].[Params]: {productId}, {productVariantId}, {quantity}");
            return await (productId, productVariantId, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể thêm số lượng sản phẩm trong kho vì số lượng bạn nhập {quantity} nhỏ hơn 0.")
                .MapAsync(async req => (product: await productRepository.FindByKeysAsync(req.productId), productVariant: await this.productVariantRepository.FindByKeysAsync(productVariantId), quantity))
                .FilterAsync(async req => await Task.FromResult(req.product != null), "Không tồn tại sản phẩm theo id đang thao tác.")
                .FilterAsync(async req => await Task.FromResult(req.productVariant != null), "Không tồn tại biến thể theo id đang thao tác.")
                .MapAsync(async req =>
                {
                    using (var transaction = await this.databaseManager.GetDatabaseContextTransactionAsync())
                    {
                        req.productVariant.InventoryQuantity += quantity;
                        await productVariantRepository.UpdateAsync(req.productVariant);

                        req.product.InventoryQuantity += quantity;
                        await productRepository.UpdateAsync(req.product);

                        await transaction.CommitAsync();
                        return req.productVariant.Id;
                    }
                });
        }

        public Task<Option<int, string>> UpdateProductVariantAnotherDiscountAsync(int productId, int productVariantId, float anotherDiscount)
        {
            throw new System.NotImplementedException();
        }

        public Task<Option<int, string>> UpdateProductVariantAnotherPriceAsync(int productId, int productVariantId, decimal anotherPrice)
        {
            throw new System.NotImplementedException();
        }

        public Task<Option<int, string>> UpdateProductVariantInventoryStatusAsync(int productId, int productVariantId, ProductVariantInventoryStatusEnum value)
        {
            throw new System.NotImplementedException();
        }

        public Task<Option<int, string>> UpdateProductVariantsAsync(ProductVariants data)
        {
            throw new System.NotImplementedException();
        }

        public Task<Option<int, string>> UpdateProductVariantStatusAsync(int productId, int productVariantId, ProductVariantStatusEnum value)
        {
            throw new System.NotImplementedException();
        }
    }
}