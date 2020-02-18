using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.IdentitiesExtension;
using Doitsu.Ecommerce.Core.IdentityManagers;
using Doitsu.Ecommerce.Core.Services;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Doitsu.Ecommerce.Core.Tests
{
    [Collection("EcommerceCoreCollection")]
    public class OrderServiceTest : BaseServiceTest<EcommerceCoreFixture>
    {
        private readonly string _poolKey = nameof(OrderServiceTest);

        public OrderServiceTest(EcommerceCoreFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
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

                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.MigrateAsync();
                await DatabaseHelper.MakeEmptyDatabase(webhost, _poolKey);

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

            // Add user and deposit to make order
            using (var scope = scopeFactory.CreateScope())
            {
                // Add Products
                var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
                var roleManager = scope.ServiceProvider.GetService<EcommerceRoleIntManager<EcommerceIdentityRole>>();
                var orderService = scope.ServiceProvider.GetService<IOrderService>();
                var user = new EcommerceIdentityUser()
                {
                    Email = "duc.tran@doitsu.tech",
                    UserName = "doitsu2014",
                    Fullname = "Trần Hữu Đức",
                    PhoneNumber = "0946680600",
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
                    TotalPrice = 100000000,
                    TotalQuantity = 1,
                    Discount = 0,
                    Note = "Nạp tiền 100k"
                });

                Assert.True(true);
            }
        }

        [System.Obsolete]
        [Fact]
        private async Task Test_CreateOrderWithProductOption_NormalProducts()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                await InitialDatabaseAsync(webhost);
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
                    var orderService = scope.ServiceProvider.GetService<IOrderService>();
                    var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");

                    var orders1 = PrepareOrders(user.Id);
                    foreach (var order in orders1)
                    {
                        (await orderService.CreateSaleOrderWithOptionAsync(order))
                            .MatchSome(res => Assert.NotNull(res));
                    }
                }
            }
        }

        [System.Obsolete]
        [Fact]
        private async Task Test_CreateOrderWithProductOption_DisabledProducts()
        {
            using (var webhost = WebHostBuilderHelper.PoolBuilderDb(_poolKey).Build())
            {
                await InitialDatabaseAsync(webhost);
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();

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
                        .Select(x => { x.Status = ProductVariantStatusEnum.Unavailable; return x; })
                        .ToImmutableList();
                    productVariantService.UpdateRange(listProductVariantOfProduct01);
                    await productVariantService.CommitAsync();
                }

                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
                    var orderService = scope.ServiceProvider.GetService<IOrderService>();
                    var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");

                    var orders1 = PrepareOrders(user.Id);
                    foreach (var order in orders1)
                    {
                        (await orderService.CreateSaleOrderWithOptionAsync(order))
                            .MatchNone(error => Assert.True(!error.IsNullOrEmpty()));
                    }
                }
            }
        }

        private List<CreateOrderWithOptionViewModel> PrepareOrders(int userId)
        {
            var listOrders = new List<CreateOrderWithOptionViewModel>()
                    {
                        new CreateOrderWithOptionViewModel()
                        {
                            UserId = userId,
                            DeliveryPhone = "0946680600",
                            DeliveryAddress = "0946680600",
                            Note = "Ghi chú sản phẩm 01",
                            Dynamic03 = "APP8239123756",
                            OrderItems = new List<CreateOrderItemWithOptionViewModel>()
                            {
                                new CreateOrderItemWithOptionViewModel()
                                {
                                    SubTotalQuantity = 1,
                                    SubTotalPrice = 100000,
                                    SubTotalFinalPrice = 100000,
                                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                                    {
                                        new ProductOptionValueViewModel()
                                        {
                                            Id = 2
                                        },
                                        new ProductOptionValueViewModel()
                                        {
                                            Id = 4
                                        },
                                        new ProductOptionValueViewModel()
                                        {
                                            Id = 6
                                        }
                                    }
                                }
                            }
                        },
                        new CreateOrderWithOptionViewModel()
                        {
                            UserId = userId,
                            DeliveryPhone = "0946680600",
                            DeliveryAddress = "0946680600",
                            Note = "Ghi chú sản phẩm 01",
                            Dynamic03 = "APP839027192",
                            OrderItems = new List<CreateOrderItemWithOptionViewModel>()
                            {
                                new CreateOrderItemWithOptionViewModel()
                                {
                                    SubTotalQuantity = 1,
                                    SubTotalPrice = 100000,
                                    SubTotalFinalPrice = 100000,
                                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                                    {
                                        new ProductOptionValueViewModel()
                                        {
                                            Id = 1
                                        },
                                        new ProductOptionValueViewModel()
                                        {
                                            Id = 4
                                        },
                                        new ProductOptionValueViewModel()
                                        {
                                            Id = 6
                                        }
                                    }
                                }
                            }
                        }
                    };
            return listOrders;
        }
    }
}
