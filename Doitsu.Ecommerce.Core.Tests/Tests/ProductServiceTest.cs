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
using Doitsu.Service.Core.Extensions;
using Optional;
using Optional.Async;

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
        private async Task CreateProductManagement()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                var dbContext = webhost.Services.GetService<EcommerceDbContext>();
                await dbContext.Database.MigrateAsync();
                DatabaseHelper.TruncateAllTable(webhost, _poolKey);

                var categoryService = webhost.Services.GetService<ICategoryService>();
                var productService = webhost.Services.GetService<IProductService>();
                await categoryService.CreateAsync<CategoryViewModel>(_fixture.CategoryData);
                await dbContext.SaveChangesAsync();

                var firstCategory = await dbContext.Set<Categories>().AsNoTracking().FirstOrDefaultAsync();
                var createData = _fixture.ProductData.Select(x => { x.CateId = firstCategory.Id; return x; });
                var result = await productService.CreateProductWithOptionAsync(createData.ToList());
                Assert.True(true);
            }
        }

        [System.Obsolete]
        [Fact]
        private async Task UpdateProductManagement()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                var dbContext = webhost.Services.GetService<EcommerceDbContext>();
                await dbContext.Database.MigrateAsync();
                DatabaseHelper.TruncateAllTable(webhost, _poolKey);

                var categoryService = webhost.Services.GetService<ICategoryService>();
                var productService = webhost.Services.GetService<IProductService>();

                await categoryService.CreateAsync<CategoryViewModel>(_fixture.CategoryData);
                await dbContext.SaveChangesAsync();

                var firstCategory = await dbContext.Set<Categories>().AsNoTracking().FirstOrDefaultAsync();
                var createData = _fixture.ProductData.Select(x => { x.CateId = firstCategory.Id; return x; });
                var firstProduct = createData.First();
                var result = await productService.CreateProductWithOptionAsync(firstProduct);
                Assert.True(true);
            }

            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                var productService = webhost.Services.GetService<IProductService>();
                var firstProductData = _fixture.ProductData.First();
                var updatedProduct = await productService.FirstOrDefaultAsync<UpdateProductViewModel>(x => x.Code == firstProductData.Code);
                updatedProduct.ProductOptions.First().ProductOptionValues.First().Status = ProductOptionValueStatusEnum.Unavailable;
                updatedProduct.ProductOptions.Last().ProductOptionValues.First().Value = "Changed";
                updatedProduct.ProductOptions.Last().ProductOptionValues.Add(new ProductOptionValueViewModel()
                {
                    Value = "Trả ông nội mày"
                });
                await productService.UpdateProductWithOptionAsync(updatedProduct);
                Assert.True(true);
            }
        }
    }
}
