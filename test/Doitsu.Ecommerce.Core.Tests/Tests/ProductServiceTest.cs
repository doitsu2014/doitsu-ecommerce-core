using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Service.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Doitsu.Ecommerce.Core.Tests
{
    [Collection("EcommerceCoreCollection")]
    public class ProductServiceTest : BaseServiceTest<EcommerceCoreFixture>
    {
        private readonly string _poolKey = nameof(ProductServiceTest);

        public ProductServiceTest(EcommerceCoreFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
        }

        private async Task InitialDatabaseAsync(IWebHost webhost)
        {
            var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                // Truncate data
                var serviceScopeFactory = scope.ServiceProvider.GetService<IServiceScopeFactory>();
                var dbContext = scope.ServiceProvider.GetService<EcommerceDbContext>();
                var categoryService = scope.ServiceProvider.GetService<ICategoryService>();
                var productService = scope.ServiceProvider.GetService<IProductService>();
                var productVariantService = scope.ServiceProvider.GetService<IProductVariantService>();
                var databaseConfigurer = scope.ServiceProvider.GetService<IDatabaseConfigurer>();


                await DatabaseHelper.MigrateDatabase(dbContext, databaseConfigurer, webhost, _poolKey);

                // Add Category
                await categoryService.CreateAsync<CategoryWithInverseParentViewModel>(_fixture.CategoryData);
                await categoryService.CommitAsync();
                // Add Products
                var firstCategory = await categoryService.GetAll().FirstOrDefaultAsync(x => x.Slug == "hang-ban");
                var productData = _fixture.ProductData.Select(x => { x.CateId = firstCategory.Id; return x; }).ToImmutableList();
                var result = await productService.CreateProductWithOptionAsync(productData);
                await productService.CommitAsync();
            }

            using (var scope = scopeFactory.CreateScope())
            {
                var categoryService = scope.ServiceProvider.GetService<ICategoryService>();
                var productService = scope.ServiceProvider.GetService<IProductService>();
                var productVariantService = scope.ServiceProvider.GetService<IProductVariantService>();

                // Add Promotion Detail
                var promotionDetailService = webhost.Services.GetService<IPromotionDetailService>();
                var listProductVariantOfProduct01 = (await productService.Get(pro => pro.Code == "GACH-CUOC")
                        .Include(p => p.ProductVariants)
                        .AsNoTracking()
                        .FirstOrDefaultAsync())
                    .ProductVariants
                    .Select(x => { x.AnotherDiscount = 25; return x; })
                    .ToImmutableList();
                productVariantService.UpdateRange(listProductVariantOfProduct01);
                await productVariantService.CommitAsync();
            }
        }

        [Fact]
        private async Task Test_GetProductVariantIdsFromFilterParamsAsync()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                await InitialDatabaseAsync(webhost);
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var productService = scope.ServiceProvider.GetService<IProductService>();
                    var logger = scope.ServiceProvider.GetService<ILogger<ProductServiceTest>>();
                    var productFilterParams = new List<ProductFilterParamViewModel>()
                    {
                        new ProductFilterParamViewModel()
                        {
                            Id = 1,
                            ProductOptions = (new List<ProductOptionFilterParamViewModel>()
                            {
                                new ProductOptionFilterParamViewModel() {
                                    Id = 1,
                                    SelectedValueId = 1
                                },
                                new ProductOptionFilterParamViewModel() {
                                    Id = 1,
                                    SelectedValueId = 4
                                },
                                new ProductOptionFilterParamViewModel() {
                                    Id = 1,
                                    SelectedValueId = 6
                                }
                            }).ToArray()
                        }
                    }.ToArray();
                    var result = await productService.GetProductVariantIdsFromProductFilterParamsAsync(productFilterParams);
                    Assert.True(result.Count() == 1);
                }
            }
        }

        [Fact]
        private async Task Test_FindProductFromOptionAsync()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                await InitialDatabaseAsync(webhost);
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var productService = scope.ServiceProvider.GetService<IProductService>();
                    var listProductOptionValues = (await productService.Get(pro => pro.Code == _fixture.ProductData.First().Code)
                        .Include(p => p.ProductOptions)
                            .ThenInclude(po => po.ProductOptionValues)
                        .FirstOrDefaultAsync())
                    .ProductOptions
                    .Select(x => productService.Mapper.Map<ProductOptionValueViewModel>(x.ProductOptionValues.First()))
                    .ToImmutableList();

                    var result = await productService.FindProductVariantFromOptionsAsync(listProductOptionValues);
                    Assert.True(result != null);
                }
            }
        }

        [Fact]
        private async Task Test_UpdateProductWithOption()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                await InitialDatabaseAsync(webhost);
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var productService = scope.ServiceProvider.GetService<IProductService>();
                    var categoryService = scope.ServiceProvider.GetService<ICategoryService>();
                    var logger = scope.ServiceProvider.GetService<ILogger<ProductServiceTest>>();

                    var firstProductData = _fixture.ProductData.First();
                    var updatedProduct = await productService.FirstOrDefaultAsync<UpdateProductViewModel>(x => x.Code == firstProductData.Code);
                    updatedProduct.Name += "-- change";
                    updatedProduct.ProductOptions.First().ProductOptionValues.First().Status = ProductOptionValueStatusEnum.Unavailable;
                    updatedProduct.ProductOptions.Last().ProductOptionValues.First().Value = "Changed";
                    updatedProduct.ProductOptions.Last().ProductOptionValues.Add(new ProductOptionValueViewModel()
                    {
                        Value = "Trả ông nội mày"
                    });
                    (await productService.UpdateProductWithOptionAsync(updatedProduct)).MatchSome(res => Assert.True(res > 0));
                }
            }
        }

        [Fact]
        private async Task Test_DeleteProductOptions()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                await InitialDatabaseAsync(webhost);
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var productService = scope.ServiceProvider.GetService<IProductService>();
                    var firstProduct = productService.GetAll<ProductDetailViewModel>().First();
                    (await productService.DeleteProductOptionByKeyAsync(firstProduct.Id, firstProduct.ProductOptions.First().Id))
                        .MatchSome(res => Assert.True(res > 0));
                }
            }
        }
    }
}