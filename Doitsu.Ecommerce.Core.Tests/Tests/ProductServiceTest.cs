using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Services;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Ecommerce.Core.ViewModels;

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

        [System.Obsolete]
        [Fact]
        private async Task Test_GetProductVariantIdsFromFilterParamsAsync()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                // Add Products
                var productService = webhost.Services.GetService<IProductService>();
                var logger = webhost.Services.GetService<ILogger<ProductServiceTest>>();
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
                                Id = 2,
                                SelectedValueId = 4
                            }
                        }).ToArray()
                    }
                }.ToArray();
                var result = await productService.GetProductVariantIdsFromProductFilterParamsAsync(productFilterParams);
                Assert.True(result.Count() == 1);
            }
        }

        [System.Obsolete]
        [Fact]
        private async Task Test_FindProductFromOptionAsync()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                // Add Products
                var productService = webhost.Services.GetService<IProductService>();
                var listProductOptionValues = (await productService.Get(pro => pro.Code == _fixture.ProductData.First().Name)
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

        [System.Obsolete]
        [Fact]
        private async Task Test_UpdateProductWithOption()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                var dbContext = webhost.Services.GetService<EcommerceDbContext>();
                await dbContext.Database.MigrateAsync();
                DatabaseHelper.TruncateAllTable(webhost, _poolKey);
                DatabaseHelper.ReseedAllTable(webhost, _poolKey);
                var categoryService = webhost.Services.GetService<ICategoryService>();
                var productService = webhost.Services.GetService<IProductService>();

                await categoryService.CreateAsync<CategoryWithInverseParentViewModel>(_fixture.CategoryData);
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
                updatedProduct.Name += "-- change";
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

        [System.Obsolete]
        [Fact]
        private async Task Test_DeleteProductOptions()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                var dbContext = webhost.Services.GetService<EcommerceDbContext>();
                await dbContext.Database.MigrateAsync();
                DatabaseHelper.TruncateAllTable(webhost, _poolKey);
                DatabaseHelper.ReseedAllTable(webhost, _poolKey);

                var categoryService = webhost.Services.GetService<ICategoryService>();
                var productService = webhost.Services.GetService<IProductService>();

                await categoryService.CreateAsync<CategoryWithInverseParentViewModel>(_fixture.CategoryData);
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
                var firstProduct = productService.GetAll<ProductDetailViewModel>().First();
                await productService.DeleteProductOptionByKeyAsync(firstProduct.Id, firstProduct.ProductOptions.First().Id);
                Assert.True(true);
            }
        }
    }
}