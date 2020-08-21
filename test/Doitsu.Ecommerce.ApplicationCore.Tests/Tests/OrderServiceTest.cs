using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Doitsu.Ecommerce.ApplicationCore.Tests
{
    [Collection("EcommerceCoreCollection")]
    public class OrderServiceTest : BaseServiceTest<EcommerceCoreFixture>
    {
        private readonly string _poolKey = nameof(OrderServiceTest);

        public OrderServiceTest(EcommerceCoreFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
            var a = "Trần Hữu Đức";
        }

        // private async Task InitialDatabaseAsync(IWebHost webhost)
        // {
        //     var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
        //     using (var scope = scopeFactory.CreateScope())
        //     {
        //         // Truncate data
        //         var serviceScopeFactory = scope.ServiceProvider.GetService<IServiceScopeFactory>();
        //         var dbContext = scope.ServiceProvider.GetService<EcommerceDbContext>();
        //         var categoryService = scope.ServiceProvider.GetService<ICategoryService>();
        //         var productService = scope.ServiceProvider.GetService<IProductService>();
        //         var brandService = scope.ServiceProvider.GetService<IBrandService>();
        //         var productVariantService = scope.ServiceProvider.GetService<IProductVariantService>();
        //         var databaseConfigurer = scope.ServiceProvider.GetService<IDatabaseConfigurer>();

        //         await DatabaseHelper.MigrateDatabase(dbContext, databaseConfigurer, webhost, _poolKey);

        //         // Add Brand
        //         await brandService.CreateAsync(_fixture.BrandData);
        //         await brandService.CommitAsync();

        //         // Add Category
        //         await categoryService.CreateAsync<CategoryWithInverseParentViewModel>(_fixture.CategoryData);
        //         await categoryService.CommitAsync();
        //         // Add Products
        //         var firstCategory = await categoryService.GetAll().FirstOrDefaultAsync(x => x.Slug == "hang-ban");
        //         var productData = _fixture.ProductData.Select(x => { x.CateId = firstCategory.Id; return x; }).ToImmutableList();
        //         var result = await productService.CreateProductWithOptionAsync(productData);
        //         await productService.CommitAsync();
        //     }

        //     using (var scope = scopeFactory.CreateScope())
        //     {
        //         var categoryService = scope.ServiceProvider.GetService<ICategoryService>();
        //         var productService = scope.ServiceProvider.GetService<IProductService>();
        //         var productVariantService = scope.ServiceProvider.GetService<IProductVariantService>();

        //         // Add Promotion Detail
        //         var promotionDetailService = webhost.Services.GetService<IPromotionDetailService>();
        //         var listProductVariantOfProduct01 = (await productService.Get(pro => pro.Code == "GACH-CUOC")
        //                 .Include(p => p.ProductVariants)
        //                 .AsNoTracking()
        //                 .FirstOrDefaultAsync())
        //             .ProductVariants
        //             .Select(x => { x.AnotherDiscount = 25; return x; })
        //             .ToImmutableList();
        //         productVariantService.UpdateRange(listProductVariantOfProduct01);
        //         await productVariantService.CommitAsync();
        //     }

        //     // Add user and deposit to make order
        //     using (var scope = scopeFactory.CreateScope())
        //     {
        //         // Add Products
        //         var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //         var roleManager = scope.ServiceProvider.GetService<EcommerceRoleIntManager<EcommerceIdentityRole>>();
        //         var orderService = scope.ServiceProvider.GetService<IOrderService>();
        //         var user = new EcommerceIdentityUser()
        //         {
        //             Email = "duc.tran@doitsu.tech",
        //             UserName = "doitsu2014",
        //             Fullname = "Trần Hữu Đức",
        //             PhoneNumber = "0946680600",
        //             Gender = (int)GenderEnum.Male
        //         };
        //         var roleStrs = new List<string>() { "ActiveUser", "Administrator" };
        //         var roles = roleStrs.Select(r => new EcommerceIdentityRole()
        //         {
        //             Name = r,
        //             NormalizedName = r
        //         }).ToList();

        //         foreach (var role in roles)
        //         {
        //             await roleManager.CreateAsync(role);
        //         }
        //         await userManager.CreateAsync(user, "zaQ@1234");
        //         await userManager.AddToRolesAsync(user, roleStrs);

        //         var result = await orderService.CreateDepositOrderAsync(new OrderViewModel()
        //         {
        //             UserId = user.Id,
        //             TotalPrice = 100000000,
        //             TotalQuantity = 1,
        //             Discount = 0,
        //             Note = "Nạp tiền 100.000.000 VND"
        //         });

        //         Assert.True(true);
        //     }
        // }

        // [Fact]
        // private async Task Test_CreateSaleOrderWithProductOption_NormalProducts()
        // {
        //     using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
        //     {
        //         await InitialDatabaseAsync(webhost);
        //         var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
        //         var orders1 = PrepareOrders(0);
        //         foreach (var order in orders1)
        //         {
        //             using (var scope = scopeFactory.CreateScope())
        //             {
        //                 // Add Products
        //                 var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //                 var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
        //                 order.UserId = user.Id;
        //                 var orderService = scope.ServiceProvider.GetService<IOrderService>();
        //                 (await orderService.CreateSaleOrderWithOptionAsync(order))
        //                     .MatchNone(error => Assert.True(error.IsNullOrEmpty()));
        //             }
        //         }
        //     }
        // }


        // [Fact]
        // private async Task Test_CreateNormalOrderWithProductOption_NormalProducts()
        // {
        //     using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
        //     {
        //         await InitialDatabaseAsync(webhost);
        //         var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
        //         var orders1 = PrepareOrdersWithManyProductInOne(0);
        //         foreach (var order in orders1)
        //         {
        //             using (var scope = scopeFactory.CreateScope())
        //             {
        //                 // Add Products
        //                 var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //                 var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
        //                 order.UserId = user.Id;
        //                 var orderService = scope.ServiceProvider.GetService<IOrderService>();
        //                 (await orderService.CreateNormalOrderWithOptionAsync(order))
        //                     .MatchNone(error => Assert.True(error.IsNullOrEmpty()));
        //             }
        //         }

        //         using (var scope = scopeFactory.CreateScope())
        //         {
        //             var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //             var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
        //             // Check user balance if not updated is good, else is not good.
        //             Assert.Equal(100000000, user.Balance);
        //         }
        //     }
        // }



        // [Fact]
        // private async Task Test_ChangeStatusOrder_Done()
        // {
        //     using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
        //     {
        //         await InitialDatabaseAsync(webhost);
        //         var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
        //         var orders1 = PrepareOrders(0);
        //         foreach (var order in orders1)
        //         {
        //             using (var scope = scopeFactory.CreateScope())
        //             {
        //                 // Add Products
        //                 var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //                 var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
        //                 var orderService = scope.ServiceProvider.GetService<IOrderService>();
        //                 order.UserId = user.Id;
        //                 (await orderService.CreateSaleOrderWithOptionAsync(order))
        //                     .MatchNone(error => Assert.True(error.IsNullOrEmpty()));
        //             }
        //         }

        //         using (var scope = scopeFactory.CreateScope())
        //         {
        //             var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //             var orderService = scope.ServiceProvider.GetService<IOrderService>();

        //             // ADMIN USER
        //             var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
        //             var orders = orderService.Get(o => o.Type == OrderTypeEnum.Sale && o.Status == (int)OrderStatusEnum.New)
        //                 .Select(o => orderService.Mapper.Map<OrderViewModel>(o))
        //                 .ToImmutableList();

        //             var summaryOrder = new CreateSummaryOrderViewModel()
        //             {
        //                 Note = "Order Summary Testing",
        //                 Orders = orders.ToList()
        //             };
        //             (await orderService.CreateSummaryOrderAsync(summaryOrder, user.Id))
        //                 .MatchNone(error => Assert.True(error.IsNullOrEmpty()));

        //         }

        //         using (var scope = scopeFactory.CreateScope())
        //         {
        //             var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //             var orderService = scope.ServiceProvider.GetService<IOrderService>();

        //             // ADMIN USER
        //             var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
        //             var order = await orderService.Get(o => o.Type == OrderTypeEnum.Sale && o.Status == (int)OrderStatusEnum.Processing)
        //                 .Select(o => orderService.Mapper.Map<OrderViewModel>(o))
        //                 .FirstOrDefaultAsync();

        //             (await orderService.ChangeOrderStatus(order.Id, OrderStatusEnum.Done, user.Id, "Complete Note"))
        //                 .MatchNone(error =>
        //                     Assert.True(error.IsNullOrEmpty()));
        //         }
        //     }
        // }

        // [Fact]
        // private async Task Test_ChangeStatusOrder_Cancel()
        // {
        //     using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
        //     {
        //         await InitialDatabaseAsync(webhost);
        //         var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
        //         var orders1 = PrepareOrders(0);
        //         foreach (var order in orders1)
        //         {
        //             using (var scope = scopeFactory.CreateScope())
        //             {
        //                 // Add Products
        //                 var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //                 var orderService = scope.ServiceProvider.GetService<IOrderService>();
        //                 var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
        //                 order.UserId = user.Id;
        //                 (await orderService.CreateSaleOrderWithOptionAsync(order))
        //                     .MatchNone(error => Assert.True(error.IsNullOrEmpty()));
        //             }
        //         }

        //         using (var scope = scopeFactory.CreateScope())
        //         {
        //             var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //             var orderService = scope.ServiceProvider.GetService<IOrderService>();

        //             // ADMIN USER
        //             var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
        //             var orderId = await orderService.Get(o => o.Type == OrderTypeEnum.Sale && o.Status == (int)OrderStatusEnum.New)
        //                 .Select(o => o.Id)
        //                 .FirstOrDefaultAsync();
        //             (await orderService.ChangeOrderStatus(orderId, OrderStatusEnum.Cancel, user.Id, "Cancel Note"))
        //                 .MatchNone(error => Assert.True(error.IsNullOrEmpty()));
        //         }

        //         Assert.True(true);
        //     }
        // }

        // [Fact]
        // private async Task Test_CreateSaleOrderWithProductOption_DisabledProducts()
        // {
        //     using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
        //     {
        //         await InitialDatabaseAsync(webhost);
        //         var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();

        //         using (var scope = scopeFactory.CreateScope())
        //         {
        //             var categoryService = scope.ServiceProvider.GetService<ICategoryService>();
        //             var productService = scope.ServiceProvider.GetService<IProductService>();
        //             var productVariantService = scope.ServiceProvider.GetService<IProductVariantService>();

        //             // Add Promotion Detail
        //             var promotionDetailService = webhost.Services.GetService<IPromotionDetailService>();
        //             var listProductVariantOfProduct01 = (await productService.Get(pro => pro.Code == "GACH-CUOC")
        //                     .Include(p => p.ProductVariants)
        //                     .AsNoTracking()
        //                     .FirstOrDefaultAsync())
        //                 .ProductVariants
        //                 .Select(x => { x.Status = ProductVariantStatusEnum.Unavailable; return x; })
        //                 .ToImmutableList();
        //             productVariantService.UpdateRange(listProductVariantOfProduct01);
        //             await productVariantService.CommitAsync();
        //         }

        //         using (var scope = scopeFactory.CreateScope())
        //         {
        //             // Add Products
        //             var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
        //             var orderService = scope.ServiceProvider.GetService<IOrderService>();
        //             var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");

        //             var orders1 = PrepareOrders(user.Id);
        //             foreach (var order in orders1)
        //             {
        //                 (await orderService.CreateSaleOrderWithOptionAsync(order))
        //                     .MatchNone(error =>
        //                         Assert.True(!error.IsNullOrEmpty()));
        //             }
        //         }
        //     }
        // }

        // private List<CreateOrderWithOptionViewModel> PrepareOrders(int userId)
        // {
        //     var listOrders = new List<CreateOrderWithOptionViewModel>()
        //             {
        //                 new CreateOrderWithOptionViewModel()
        //                 {
        //                     UserId = userId,
        //                     DeliveryPhone = "0946680600",
        //                     DeliveryAddress = "0946680600",
        //                     Note = "Ghi chú sản phẩm 01",
        //                     Dynamic03 = "APP8239123756",
        //                     OrderItems = new List<CreateOrderItemWithOptionViewModel>()
        //                     {
        //                         new CreateOrderItemWithOptionViewModel()
        //                         {
        //                             SubTotalQuantity = 1,
        //                             SubTotalPrice = 100000,
        //                             SubTotalFinalPrice = 100000,
        //                             ProductOptionValues = new List<ProductOptionValueViewModel>()
        //                             {
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 2
        //                                 },
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 4
        //                                 },
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 6
        //                                 }
        //                             }
        //                         }
        //                     }
        //                 },
        //                 new CreateOrderWithOptionViewModel()
        //                 {
        //                     UserId = userId,
        //                     DeliveryPhone = "0946680600",
        //                     DeliveryAddress = "0946680600",
        //                     Note = "Ghi chú sản phẩm 01",
        //                     Dynamic03 = "APP839027192",
        //                     OrderItems = new List<CreateOrderItemWithOptionViewModel>()
        //                     {
        //                         new CreateOrderItemWithOptionViewModel()
        //                         {
        //                             SubTotalQuantity = 1,
        //                             SubTotalPrice = 100000,
        //                             SubTotalFinalPrice = 100000,
        //                             ProductOptionValues = new List<ProductOptionValueViewModel>()
        //                             {
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 1
        //                                 },
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 4
        //                                 },
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 6
        //                                 }
        //                             }
        //                         }
        //                     }
        //                 }
        //             };
        //     return listOrders;
        // }

        // private List<CreateOrderWithOptionViewModel> PrepareOrdersWithManyProductInOne(int userId)
        // {
        //     var listOrders = new List<CreateOrderWithOptionViewModel>()
        //             {
        //                 new CreateOrderWithOptionViewModel()
        //                 {
        //                     UserId = userId,
        //                     DeliveryPhone = "0946680600",
        //                     DeliveryAddress = "0946680600",
        //                     Note = "Ghi chú sản phẩm 01",
        //                     Dynamic03 = "APP8239123756",
        //                     OrderItems = new List<CreateOrderItemWithOptionViewModel>()
        //                     {
        //                         new CreateOrderItemWithOptionViewModel()
        //                         {
        //                             SubTotalQuantity = 1,
        //                             SubTotalPrice = 100000,
        //                             SubTotalFinalPrice = 100000,
        //                             ProductOptionValues = new List<ProductOptionValueViewModel>()
        //                             {
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 2
        //                                 },
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 4
        //                                 },
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 6
        //                                 }
        //                             }
        //                         },
        //                         new CreateOrderItemWithOptionViewModel()
        //                         {
        //                             SubTotalQuantity = 1,
        //                             SubTotalPrice = 100000,
        //                             SubTotalFinalPrice = 100000,
        //                             ProductOptionValues = new List<ProductOptionValueViewModel>()
        //                             {
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 1
        //                                 },
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 4
        //                                 },
        //                                 new ProductOptionValueViewModel()
        //                                 {
        //                                     Id = 6
        //                                 }
        //                             }
        //                         }
        //                     }
        //                 }
        //             };
        //     return listOrders;
        // }
    
    
    
    }
}
