using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Specifications.ProductVariantSpecifications;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices
{
    public partial class ProductBusinessService : IProductBusinessService
    {
        private readonly IBaseEcommerceRepository<Products> productRepository;
        private readonly IBaseEcommerceRepository<ProductVariants> productVariantRepository;
        private readonly IBaseEcommerceRepository<ProductOptions> productOptionRepository;
        private readonly IBaseEcommerceRepository<ProductOptionValues> productOptionValueRepository;
        private readonly IBaseEcommerceRepository<ProductVariantOptionValues> productVariantOptionValueRepository;
        private readonly IEcommerceDatabaseManager databaseManager;
        private readonly ILogger<ProductBusinessService> logger;

        public ProductBusinessService(IBaseEcommerceRepository<Products> productRepository,
                                      IBaseEcommerceRepository<ProductVariants> productVariantRepository,
                                      IBaseEcommerceRepository<ProductOptions> productOptionRepository,
                                      IBaseEcommerceRepository<ProductOptionValues> productOptionValueRepository,
                                      IBaseEcommerceRepository<ProductVariantOptionValues> productVariantOptionValueRepository,
                                      IEcommerceDatabaseManager databaseManager,
                                      ILogger<ProductBusinessService> logger)
        {
            this.databaseManager = databaseManager;
            this.logger = logger;
            this.productVariantRepository = productVariantRepository;
            this.productOptionRepository = productOptionRepository;
            this.productOptionValueRepository = productOptionValueRepository;
            this.productVariantOptionValueRepository = productVariantOptionValueRepository;
            this.productRepository = productRepository;
        }

        public async Task<Option<int, string>> CreateProductWithOptionAsync(Products data)
        {
            return await data.SomeNotNull()
                .WithException("Dữ liệu truyền vào bị rỗng")
                .MapAsync(async d =>
                {
                    using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
                    {
                        data.ProductVariants = this.BuildListProductVariant(data.Id, data.Code, data.ProductOptions.ToArray(), data.ProductVariants.ToArray());
                        var created = await productRepository.AddAsync(data);
                        await transaction.CommitAsync();
                        return created.Id;
                    }
                });
        }

        public async Task<Option<int[], string>> CreateProductWithOptionAsync(Products[] data)
        {
            using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
            {
                return await data.SomeNotNull()
                    .WithException("Dữ liệu truyền vào bị rỗng")
                    .MapAsync(async d =>
                    {
                        var result = new int[] { };
                        var created = await productRepository.AddRangeAsync(d);
                        created.ToList().ForEach(cP => 
                        {
                            result = result.Append(cP.Id).ToArray();
                            cP.ProductVariants = this.BuildListProductVariant(cP.Id, cP.Code, cP.ProductOptions.ToArray(), cP.ProductVariants?.ToArray());
                        });
                        await productRepository.UpdateRangeAsync(created);
                        await transaction.CommitAsync();
                        return result;
                    });
            }
        }


        public async Task<Option<int, string>> IncreaseInventoryQuantity(int productId, int quantity = 0)
        {
            return await (productId, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể thêm số lượng sản phẩm trong kho vì số lượng bạn nhập {quantity} nhỏ hơn 0.")
                .MapAsync(async req => (product: await productRepository.FindByKeysAsync(req.productId), quantity))
                .FilterAsync(async req => await Task.FromResult(req.product != null), "Không tồn tại sản phẩm theo id đang thao tác.")
                .MapAsync(async req =>
                {
                    req.product.InventoryQuantity += quantity;
                    await productRepository.UpdateAsync(req.product);
                    return req.product.Id;
                });
        }

        public async Task<Option<int, string>> DecreaseInventoryQuantity(int productId, int quantity = 0)
        {
            return await (productId, quantity).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => quantity >= 0, $"Không thể giảm số lượng sản phẩm trong kho vì số lượng truyền vào {quantity} nhỏ hơn 0.")
                .MapAsync(async req => (product: await productRepository.FindByKeysAsync(req.productId), quantity))
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
                        await productRepository.UpdateAsync(req.product);
                        return Option.Some<int, string>(req.product.Id);
                    }
                });
        }

        public async Task<Option<int, string>> UpdateProductAndRelationAsync(Products data)
        {
            return await data
                .SomeNotNull()
                .WithException("Dữ liệu truyền vào bị rỗng")
                .MapAsync(async d =>
                {
                    // Modified Product Options
                    var listPoForUpdateAction = d.ProductOptions.Where(po => po.Id != 0).ToArray();
                    await productOptionRepository.UpdateRangeAsync(listPoForUpdateAction);
                    var listNewPoForCreateAction = d.ProductOptions.Where(po => po.Id == 0).ToArray();
                    await productOptionRepository.AddRangeAsync(listNewPoForCreateAction);

                    // Generate and modify product variants
                    this.BuildListProductVariant(d.Id, d.Code, d.ProductOptions.ToArray(), d.ProductVariants.ToArray())
                        .ForEach(gPv =>
                        {
                            var existPv = d.ProductVariants.FirstOrDefault(pv => pv.Id == gPv.Id && pv.Id > 0);
                            if (existPv != null)
                            {
                                // modified variants
                                existPv.Sku = gPv.Sku;
                            }
                            else
                            {
                                // Add new variant
                                d.ProductVariants.Add(gPv);
                            }
                        });

                    // Make product variant have any unavailable product option value to unavailable
                    var listPoUnavaiable = d.ProductOptions
                        .SelectMany(po => po.ProductOptionValues)
                        .Where(pov => pov.Status == ProductOptionValueStatusEnum.Unavailable)
                        .Select(x => x.Id)
                        .Distinct()
                        .ToImmutableList();

                    foreach (var pva in d.ProductVariants)
                    {
                        var isUnavailable = pva.ProductVariantOptionValues.Any(pvov => listPoUnavaiable.Contains(pvov.ProductOptionValueId ?? int.MinValue));
                        if (isUnavailable)
                        {
                            pva.Status = ProductVariantStatusEnum.Unavailable;
                        }
                        else
                        {
                            pva.Status = ProductVariantStatusEnum.Available;
                        }
                    }

                    await this.productRepository.UpdateAsync(d);
                    return d.Id;
                });
        }

        public async Task<Option<int, string>> DeleteProductOptionByKeyAsync(int productId, int productOptionId)
        {
            using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
            {
                return await (productId, productOptionId)
                    .SomeNotNull()
                    .WithException("Id rỗng.")
                    .FilterAsync(async d => !(await productOptionRepository.AnyAsync(new ProductOptionFilterByIdAndExistOrderItemsSpecification(d.productOptionId))), "Không thể xoá thuộc tính này vì đang có đơn hàng trực thuộc nó.")
                    .MapAsync(async req =>
                    {
                        var po = await productOptionRepository.FirstOrDefaultAsync(new ProductOptionFilterByIdSpecification(req.productOptionId));
                        var listPvForDeleteAction = po.ProductOptionValues.SelectMany(pov => pov.ProductVariantOptionValues.Select(pvov => pvov.ProductVariant)).ToArray();
                        // var listPovForDeleteAction = po.ProductOptionValues.ToArray();
                        // var listPvovForDeleteAction = po.ProductOptionValues.SelectMany(pov => pov.ProductVariantOptionValues).ToArray();

                        // await productVariantOptionValueRepository.DeleteRangeAsync(listPvovForDeleteAction, isForcedMode: true);
                        await productVariantRepository.DeleteRangeAsync(listPvForDeleteAction, isForcedMode: true);

                        // await productOptionValueRepository.DeleteRangeAsync(listPovForDeleteAction, isForcedMode: true);
                        await productOptionRepository.DeleteAsync(po, isForcedMode: true);

                        return req;
                    })
                    .MapAsync(async req =>
                    {
                        var productEnt = await productRepository.FirstOrDefaultAsync(new ProductFilterByIdsSpecification(req.productId));
                        var listGeneratedProductVariants = this.BuildListProductVariant(productEnt.Id,
                                                                                        productEnt.Code,
                                                                                        productEnt.ProductOptions.ToArray(),
                                                                                        productEnt.ProductVariants.ToArray());
                        await productVariantRepository.AddRangeAsync(listGeneratedProductVariants.Where(gPv => gPv.Id == 0).ToArray());

                        await transaction.CommitAsync();
                        return req.productOptionId;
                    });
            }
        }
    }
}