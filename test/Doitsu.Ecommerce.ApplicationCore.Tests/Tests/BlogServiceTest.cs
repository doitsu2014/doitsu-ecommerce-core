using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Xunit.Abstractions;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Doitsu.Ecommerce.Infrastructure.Data;
using Doitsu.Ecommerce.ApplicationCore.Services.IdentityManagers;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using System.Collections.Generic;
using System;

namespace Doitsu.Ecommerce.ApplicationCore.Tests
{
    [Collection("EcommerceCoreCollection")]
    public class BlogServiceTest : BaseServiceTest<EcommerceCoreFixture>
    {
        private readonly string _poolKey = nameof(BlogServiceTest);

        public BlogServiceTest(EcommerceCoreFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
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
                await DatabaseHelper.MigrateDatabase(dbContext, databaseConfigurer, webhost, _poolKey);
            }

            using (var scope = scopeFactory.CreateScope())
            {
                // Add Products
                var blogCategoryRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<BlogCategories>>();
                var createdBlogCategories = await blogCategoryRepository.AddRangeAsync(new BlogCategories[]
                {
                    new BlogCategories() { Name = "Category 01", Slug = "category-01", Position = 1, CreatedDate = DateTime.Now, UpdatedTime = DateTime.Now }
                });
            }

            using (var scope = scopeFactory.CreateScope())
            {
                // Add Products
                var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
                var roleManager = scope.ServiceProvider.GetService<EcommerceRoleIntManager<EcommerceIdentityRole>>();
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
                Assert.True(true);
            }
        }

        [Fact]
        private async Task Test_CreateBlogWithTags()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                await InitialDatabaseAsync(webhost);
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var blogBusinessService = scope.ServiceProvider.GetService<IBlogBusinessService>();
                    var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
                    var blogCategoryRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<BlogCategories>>();

                    var listBlogCategories = await blogCategoryRepository.ListAllAsync();
                    var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
                    var createdEntity = await blogBusinessService.CreateBlogWithTagsAsync(MockBlog(listBlogCategories.First().Id, user.Id), MockTags());
                    Assert.True(createdEntity.Id > 0);
                }
            }
        }

        [Fact]
        private async Task Test_UpdateBlogWithTags()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                await InitialDatabaseAsync(webhost);
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var blogRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<Blogs>>();
                    var blogBusinessService = scope.ServiceProvider.GetService<IBlogBusinessService>();
                    var userManager = scope.ServiceProvider.GetService<EcommerceIdentityUserManager<EcommerceIdentityUser>>();
                    var blogCategoryRepository = scope.ServiceProvider.GetService<IBaseEcommerceRepository<BlogCategories>>();

                    var listBlogCategories = await blogCategoryRepository.ListAllAsync();
                    var user = await userManager.FindByEmailAsync("duc.tran@doitsu.tech");
                    var createdEntity = await blogBusinessService.CreateBlogWithTagsAsync(MockBlog(listBlogCategories.First().Id, user.Id), MockTags());
                    Assert.True(createdEntity.Id > 0);

                    var listUpdatingTags = MockTags();
                    listUpdatingTags = listUpdatingTags.Append(new Tag()
                    {
                        Title = "Addition",
                        Slug = "additional"
                    }).ToArray();
                    await blogBusinessService.UpdateBlogWithTagsAsync(createdEntity, listUpdatingTags);
                    var updatedBlog = await blogRepository.FindByKeysAsync(createdEntity.Id);

                    Assert.True(updatedBlog.BlogTags.Count == listUpdatingTags.Count());

                    var aggregatedUpdatingTagTitles = listUpdatingTags.Select(x => x.Title).OrderBy(x => x).Aggregate((aa,bb) => $"{aa},{bb}");
                    var aggregatedUpdatedBlogTags = updatedBlog.BlogTags.Select(x => x.Tag.Title).OrderBy(x => x).Aggregate((aa,bb) => $"{aa},{bb}");
                    Assert.True(aggregatedUpdatedBlogTags == aggregatedUpdatingTagTitles);
                }
            }
        }

        private Blogs MockBlog(int blogCategoryId, int userId) => new Blogs()
        {
            Title = "Blog Title 01",
            BlogCategoryId = blogCategoryId,
            CreaterId = userId,
            ShortContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            Slug = "blog-title-01",
            ThumbnailUrl = "https://image.com/12345.jpg",
            PublisherId = userId,
            DraftedTime = DateTime.Now,
            PublishedTime = DateTime.Now
        };

        private Tag[] MockTags() => new Tag[]
        {
            new Tag() { Title = "Animation", Slug = "animation" },
            new Tag() { Title = "Testing", Slug = "testing" },
            new Tag() { Title = "Chicken Dry", Slug = "chicken-dry" },
        };
    }
}