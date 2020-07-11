using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Ecommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Specifications.CategorySpecifications;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    [Collection("EcommerceCoreCollection")]
    public class CategoryServiceTest : BaseServiceTest<EcommerceCoreFixture>
    {
        private readonly string _poolKey = nameof(CategoryServiceTest);

        public CategoryServiceTest(EcommerceCoreFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
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
                await DatabaseHelper.MigrateDatabase(dbContext, databaseConfigurer, webhost, _poolKey);

                // Add Category
                var createdCategories = await categoryRepository.AddRangeAsync(_fixture.CategoryData.ToArray());

                // Add Products
                var categoryFilterSpecification = new CategoryFilterSpecification(TypeOfCategoryEnum.Normal, "hang-ban-1");
                var firstCategory = await categoryRepository.FirstOrDefaultAsync(categoryFilterSpecification);

                var productData = _fixture.ProductData.Select(x => { x.CateId = firstCategory.Id; return x; }).ToArray();
                var result = await productRepository.AddRangeAsync(productData);
            }
        }

        [Fact]
        private async Task Test_QueryCategoryWithLimitListProduct()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithRealDb(_poolKey).Build())
            {
                await InitialDatabaseAsync(webhost);
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    int limit = 2;
                    var categoryRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Categories>>();
                    var parentCategorySlug = "san-pham";
                    var firstParentCategory = await categoryRepository.FirstAsync(new CategoryFilterSpecification(TypeOfCategoryEnum.Parent, parentCategorySlug));
                    var normalSlugs = firstParentCategory.InverseParentCate.Select(x => x.Slug).ToArray();
                    var ctorSlugsAndLimit = new CategoryFilterAsNormalCategoryWithLimitListProductsSpecification(normalSlugs, limit);
                    var ctorParentCategorySlugAndLimit = new CategoryFilterAsNormalCategoryWithLimitListProductsSpecification(parentCategorySlug, limit);
                    var categories1 = await categoryRepository.ListAsync<Categories>(ctorSlugsAndLimit);
                    var categories2 = await categoryRepository.ListAsync<Categories>(ctorParentCategorySlugAndLimit);

                    Assert.True(categories1.First().Products.Count == limit);
                    Assert.True(categories2.First().Products.Count == limit);
                }
            }
        }
    }
}