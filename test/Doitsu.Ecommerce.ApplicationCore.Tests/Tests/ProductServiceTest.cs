using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Models.ViewModels;
using Doitsu.Ecommerce.ApplicationCore.Specifications.ProductSpecifications;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Ecommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
                var productBusinessService = scope.ServiceProvider.GetService<IProductBusinessService>();
                await DatabaseHelper.MigrateDatabase(dbContext, databaseConfigurer, webhost, _poolKey);

                // Add Categories
                var createdCategories = await categoryRepository.AddRangeAsync(_fixture.GetCategoryData().ToArray());

                // Add Products
                var categoryFilterSpecification = new CategoryFilterSpecification(TypeOfCategoryEnum.Normal, "hang-ban-1");
                var firstCategory = await categoryRepository.FirstOrDefaultAsync(categoryFilterSpecification);

                var productData = _fixture.GetProductsData().Select(x => { x.CateId = firstCategory.Id; return x; }).ToArray();
                var result = await productBusinessService.CreateProductWithOptionAsync(productData);

            }

            using (var scope = scopeFactory.CreateScope())
            {
                var productRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Products>>();
                var productVariantRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<ProductVariants>>();

                // Add Promotion Detail
                var product01 = (await productRepository.FirstOrDefaultAsync(new ProductFilterByCodeSpecification("PV934581023901")));
                var listProductVariantOfProduct01 = product01.ProductVariants
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
                    var productRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Products>>();
                    var productVariantRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<ProductVariants>>();
                    var productOptionsRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<ProductOptions>>();
                    var productBusinessService = scope.ServiceProvider.GetService<IProductBusinessService>();

                    var productVariant = await productVariantRepository.FirstOrDefaultAsync(new ProductVariantWithPvovSpecification());
                    var povIds = productVariant.ProductVariantOptionValues.Select(pvov => pvov.ProductOptionValueId.Value).ToArray();
                    var result = await productVariantRepository.ListAsync(new ProductVariantFilterByProductOptionValueIdsSpecification(povIds));
                    Assert.True(result.Count() == 1);
                }
            }
        }

        [Fact]
        private async Task Test_FindProductVariantFromOptionAsync()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                await InitialDatabaseAsync(webhost);
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var productRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Products>>();
                    var productVariantRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<ProductVariants>>();
                    var productBusinessService = scope.ServiceProvider.GetService<IProductBusinessService>();

                    var arrayProductOptionIds = (await productRepository.FirstOrDefaultAsync(new ProductFilterByCodeSpecification(_fixture.GetProductsData().First().Code)))
                        .ProductOptions
                        .Select(po => po.ProductOptionValues.First().Id)
                        .ToArray();

                    var result = await productVariantRepository.FirstOrDefaultAsync(new ProductVariantFilterByProductOptionValueIdsSpecification(arrayProductOptionIds));
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
                    var productRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Products>>();
                    var productBusinessService = scope.ServiceProvider.GetService<IProductBusinessService>();

                    var firstProductData = _fixture.GetProductsData().First();
                    var updatedProduct = await productRepository.FirstOrDefaultAsync(new ProductFilterByCodeSpecification(firstProductData.Code));
                    updatedProduct.Name += "-- change";
                    updatedProduct.ProductOptions.First().ProductOptionValues.First().Status = ProductOptionValueStatusEnum.Unavailable;
                    updatedProduct.ProductOptions.Last().ProductOptionValues.First().Value = "Changed Product Option Value Testing";
                    updatedProduct.ProductOptions.Last().ProductOptionValues.Add(new ProductOptionValues()
                    {
                        Value = "New Product Option Value Testing"
                    });

                    (await productBusinessService.UpdateProductAndRelationAsync(updatedProduct))
                        .MatchSome(res => Assert.True(res > 0));
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
                    var productRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Products>>();
                    var productBusinessService = scope.ServiceProvider.GetService<IProductBusinessService>();
                    var firstFixtureProductData = this._fixture.GetProductsData().First();
                    var firstProduct = await productRepository.FirstOrDefaultAsync(new ProductFilterByCodeSpecification(firstFixtureProductData.Code));
                    (await productBusinessService.DeleteProductOptionByKeyAsync(firstProduct.Id, firstProduct.ProductOptions.First().Id))
                        .MatchSome(res => Assert.True(res > 0));
                }
            }
        }
    }
}