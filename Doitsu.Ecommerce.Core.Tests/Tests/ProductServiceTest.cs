﻿using System.Linq;
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
using System.Collections.Immutable;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.IdentitiesExtension;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.IdentityManagers;

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
        private async Task Test_InitialData()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                // Truncate data
                var dbContext = webhost.Services.GetService<EcommerceDbContext>();
                await dbContext.Database.MigrateAsync();
                DatabaseHelper.TruncateAllTable(webhost, _poolKey);

                // Add Brand
                var brandService = webhost.Services.GetService<IBrandService>();
                await brandService.CreateAsync<BrandViewModel>(_fixture.BrandData);

                // Add Category
                var categoryService = webhost.Services.GetService<ICategoryService>();
                await categoryService.CreateAsync<CategoryViewModel>(_fixture.CategoryData);
                await dbContext.SaveChangesAsync();

                // Add Products
                var productService = webhost.Services.GetService<IProductService>();
                var firstCategory = await dbContext.Set<Categories>().AsNoTracking().FirstOrDefaultAsync();
                var createData = _fixture.ProductData.Select(x => { x.CateId = firstCategory.Id; return x; });
                var result = await productService.CreateProductWithOptionAsync(createData.ToList());
                await dbContext.SaveChangesAsync();
                Assert.True(true);
            }

            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                // Add Products
                var dbContext = webhost.Services.GetService<EcommerceDbContext>();
                var productService = webhost.Services.GetService<IProductService>();
                // Add Promotion Detail
                var promotionDetailService = webhost.Services.GetService<IPromotionDetailService>();
                var listProductVariantIdOfProduct01 = (await productService.Get(pro => pro.Code == "PRODUCT01")
                    .Include(p => p.ProductVariants)
                    .FirstOrDefaultAsync())
                    .ProductVariants
                    .Select(x => productService.Mapper.Map<ProductVariantViewModel>(x))
                    .ToImmutableList();

                var listPromotionDetailViewModel = listProductVariantIdOfProduct01.Select(pvId => new PromotionDetailViewModel()
                {
                    ProductVariantId = pvId.Id,
                    Name = $"PRODUCT01-{pvId.Id}",
                    DiscountPercent = 25
                });

                await promotionDetailService.CreateAsync(listPromotionDetailViewModel);
                await dbContext.SaveChangesAsync();
            }

            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                // Add Products
                var userManager = webhost.Services.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
                var roleManager = webhost.Services.GetService<EcommerceRoleIntManager<EcommerceIdentityRole>>();
                var orderService = webhost.Services.GetService<IOrderService>();
                var user = new EcommerceIdentityUser()
                {
                    Email = "duc.tran@doitsu.tech",
                    UserName = "doitsu2014",
                    Fullname = "Trần Hữu Đức",
                    Gender = (int)GenderEnum.Male
                };
                var roleStrs = new List<string>() { "ActiveUser", "Administrator" };
                var roles = roleStrs.Select(r => new EcommerceIdentityRole()
                {
                    Name = r,
                    NormalizedName = r
                }).ToList();
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
                await userManager.CreateAsync(user, "zaQ@1234");
                await userManager.AddToRolesAsync(user, roleStrs);

                var result = await orderService.CreateDepositOrderAsync(new OrderViewModel()
                {
                    UserId = user.Id,
                    TotalPrice = 100000,
                    TotalQuantity = 1,
                    Discount = 0,
                    Note = "Nạp tiền 100k"
                });

                Assert.True(true);
            }
        }

        [System.Obsolete]
        [Fact]
        private async Task Test_ProductManagement()
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