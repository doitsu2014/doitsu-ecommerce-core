using System.Linq;
using System.Threading.Tasks;

using Doitsu.Ecommerce.Core.Data;
using Microsoft.EntityFrameworkCore;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
namespace Doitsu.Ecommerce.Core.Tests
{
    [Collection("ProductServiceCollections")]
    public class ProductServiceTest : BaseServiceTest<ProductServiceFixture>
    {
        private readonly string _poolKey;

        public ProductServiceTest(ProductServiceFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
            _poolKey = _fixture.ServicePoolKey;
        }

        [System.Obsolete]
        [Fact]
        private async Task CreateProduct()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                var dbContext = webhost.Services.GetService<EcommerceDbContext>();
                await dbContext.Database.MigrateAsync();

                DatabaseHelper.TruncateAllTable(webhost, _poolKey);
                await AddCategories(dbContext);
                await dbContext.SaveChangesAsync();

                var firstCategory = await dbContext.Set<Categories>().FirstOrDefaultAsync();
                await AddProduct(dbContext, firstCategory.Id);
                await dbContext.SaveChangesAsync();

                Assert.True(true);
            }
        }

        private async Task AddCategories(EcommerceDbContext dbContext)
        {
            await dbContext.Set<Categories>().AddRangeAsync(_fixture.CategoryData);

        }

        private async Task AddProduct(EcommerceDbContext dbContext, int categoryId)
        {
            var listProducts = _fixture.ProductData.Select(prod => {prod.CateId = categoryId; return prod;});
            await dbContext.Set<Products>().AddRangeAsync(listProducts);
        }

    }
}
