using Doitsu.Ecommerce.Core.AuthorizeBuilder;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.IdentitiesExtension;
using Doitsu.Service.Core;
using Doitsu.Service.Core.Extensions;
using Doitsu.Service.Core.Services.EmailService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.Memory;

namespace Doitsu.Ecommerce.Core
{
    /// <summary>
    /// Is the api endpoint config to help build a web app fastly
    /// The core destination is: 
    /// + Config Identity with JWT
    /// + Config Main DB context
    /// + Config Service Dependency
    /// + Config Repo Dependency
    /// + Config Autmapper
    /// How to use:
    /// + Use should add to your config
    /// ++ Doitsu.Identity.DevDB key, value
    /// ++ Doitsu.Ecommerce.Core.DevDB key, value
    /// </summary>
    public static class RootConfig
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void AppHosting(IApplicationBuilder app, IHostingEnvironment env, bool isConfigImageSharpWeb = false)
        {
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseResponseCompression();
            if (isConfigImageSharpWeb)
                app.UseImageSharp();
        }

        public static void Service(IServiceCollection services, IConfiguration configuration, bool isConfigImageSharpWeb = false)
        {
            #region Identity Database Config
            var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
            // Config identity db config
            services.AddDbContext<EcommerceDbContext>(options =>
                    options
                    .UseSqlServer(configuration.GetConnectionString("Ecommerce.DBConStr"))
                    .UseLoggerFactory(loggerFactory)
                    .EnableSensitiveDataLogging())
                .AddIdentity<EcommerceIdentityUser, EcommerceIdentityRole>()
                .AddEntityFrameworkStores<EcommerceDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped(typeof(DbContext), typeof(EcommerceDbContext));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            // Inject Identity Manager
            services.AddScoped(typeof(EcommerceIdentityUserManager<EcommerceIdentityUser>));
            services.AddScoped(typeof(EcommerceRoleIntManager<EcommerceIdentityRole>));
            services.AddScoped(typeof(EcommerceUserSignInManager<EcommerceIdentityUser>));
            services.AddScoped(typeof(IUserClaimsPrincipalFactory<EcommerceIdentityUser>), typeof(DoitsuCookieIdenittyCustomClaimsFactory<EcommerceIdentityUser>));

            // Authentication configure
            services.AddDoitsuBasicAuthorize();
            #endregion

            #region Config service
            services.AddFurnitureServices();
            #endregion

            #region Mapper Config
            services.AddFurnitureAutoMapper();
            #endregion

            #region Cache Config
            services.AddMemoryCache();
            #endregion

            #region Compression
            services.AddGzip();
            #endregion

            if (isConfigImageSharpWeb)
            {
                ConfigureCustomServicesAndCustomOptions(services);
            }

            services.Configure<SmtpMailServerOptions>(configuration.GetSection("SmtpMailServerOptions"));
            services.Configure<LeaderMail>(configuration.GetSection("LeaderEmail"));
            services.AddDoitsuEmailService();

        }
        #region Config Image Sharp Methods
        private static void ConfigureCustomServicesAndCustomOptions(IServiceCollection services)
        {
            services.AddImageSharpCore(
                    options =>
                    {
                        options.Configuration = Configuration.Default;
                        options.MaxBrowserCacheDays = 7;
                        options.MaxCacheDays = 365;
                        options.CachedNameLength = 8;
                        options.OnParseCommands = _ => { };
                        options.OnBeforeSave = _ => { };
                        options.OnProcessed = _ => { };
                        options.OnPrepareResponse = _ => { };
                    })
                .SetRequestParser<QueryCollectionRequestParser>()
                .SetMemoryAllocator(provider => ArrayPoolMemoryAllocator.CreateWithAggressivePooling())
                .SetCache(provider =>
                {
                    var p = new PhysicalFileSystemCache(
                        provider.GetRequiredService<IHostingEnvironment>(),
                        provider.GetRequiredService<MemoryAllocator>(),
                        provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>());

                    p.Settings[PhysicalFileSystemCache.Folder] = PhysicalFileSystemCache.DefaultCacheFolder;
                    return p;
                })
                .SetCacheHash<CacheHash>()
                .AddProvider<PhysicalFileSystemProvider>()
                .AddProcessor<ResizeWebProcessor>()
                .AddProcessor<FormatWebProcessor>()
                .AddProcessor<BackgroundColorWebProcessor>();
        }
        #endregion

    }
}