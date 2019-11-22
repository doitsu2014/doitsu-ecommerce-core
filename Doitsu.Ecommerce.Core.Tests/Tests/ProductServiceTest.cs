using System.Linq;
using System.Threading.Tasks;

using Doitsu.Ecommerce.Core.Data;
using Microsoft.EntityFrameworkCore;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
using Doitsu.Ecommerce.Core.Services;
using Doitsu.Ecommerce.Core.ViewModels;

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
                var categoryService = webhost.Services.GetService<ICategoryService>();
                var productService = webhost.Services.GetService<IProductService>();
                await dbContext.Database.MigrateAsync();

                DatabaseHelper.TruncateAllTable(webhost, _poolKey);
                await categoryService.CreateAsync<CategoryViewModel>(_fixture.CategoryData);
                await dbContext.SaveChangesAsync();
                var firstCategory = await dbContext.Set<Categories>().FirstOrDefaultAsync();

                var createData = _fixture.ProductData.Select(x => { x.CateId = firstCategory.Id; return x; });
                await productService.CreateProductWithOption(createData.FirstOrDefault());
                await dbContext.SaveChangesAsync();

                Assert.True(true);
            }
        }

    }
}
