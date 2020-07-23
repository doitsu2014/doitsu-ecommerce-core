﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Specifications.ProductVariantSpecifications;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Ecommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Specifications.CategorySpecifications;
using Xunit;
using Xunit.Abstractions;

namespace Doitsu.Ecommerce.ApplicationCore.Tests
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
                var serviceScopeFactory = scope.ServiceProvider.GetService<IServiceScopeFactory>();
                var dbContext = scope.ServiceProvider.GetService<EcommerceDbContext>();
                var databaseConfigurer = scope.ServiceProvider.GetService<IDatabaseConfigurer>();
                var categoryRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Categories>>();
                var productRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Products>>();
                var productBusinessService = scope.ServiceProvider.GetService<ProductBusinessService>();
                await DatabaseHelper.MigrateDatabase(dbContext, databaseConfigurer, webhost, _poolKey);

                // Add Categories
                var createdCategories = await categoryRepository.AddRangeAsync(_fixture.CategoryData.ToArray());

                // Add Products
                var categoryFilterSpecification = new CategoryFilterSpecification(TypeOfCategoryEnum.Normal, "hang-ban-1");
                var firstCategory = await categoryRepository.FirstOrDefaultAsync(categoryFilterSpecification);

                var productData = _fixture.ProductData.Select(x => { x.CateId = firstCategory.Id; return x; }).ToArray();
                var result = await productBusinessService.CreateProductWithOptionAsync(productData);

            }

            using (var scope = scopeFactory.CreateScope())
            {
                var productRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Products>>();
                var productVariantRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<ProductVariants>>();
                var promotionDetailRepository = webhost.Services.GetService<IBaseEcommerceRepository<ProductVariants>>();

                // Add Promotion Detail
                var listProductVariantOfProduct01 = (await productRepository.FirstOrDefaultAsync(new ProductFilterByCodeSpecification("san-pham-ao-01")))
                    .ProductVariants
                    .Select(x => { x.AnotherDiscount = 25; return x; })
                    .ToImmutableList();

                await productVariantRepository.UpdateRangeAsync(listProductVariantOfProduct01.ToArray());
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
                    var productVariantService = scope.ServiceProvider.GetService<IProductVariantService>();
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
                    var result = await productVariantService.GetProductVariantIdsFromProductFilterParamsAsync(productFilterParams);
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
                    var productVariantService = scope.ServiceProvider.GetService<IProductVariantService>();
                    var listProductOptionValues = (await productService.Get(pro => pro.Code == _fixture.ProductData.First().Code)
                        .Include(p => p.ProductOptions)
                            .ThenInclude(po => po.ProductOptionValues)
                        .FirstOrDefaultAsync())
                    .ProductOptions
                    .Select(x => productService.Mapper.Map<ProductOptionValueViewModel>(x.ProductOptionValues.First()))
                    .ToImmutableList();

                    var result = await productVariantService.FindProductVariantFromOptionsAsync(listProductOptionValues);
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