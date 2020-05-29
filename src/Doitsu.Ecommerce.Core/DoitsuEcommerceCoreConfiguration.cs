using System.Collections.Generic;
using System.Globalization;
using Doitsu.Ecommerce.Core.AuthorizeBuilder;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Ecommerce.Core.IdentitiesExtension;
using Doitsu.Ecommerce.Core.IdentityManagers;
using Doitsu.Service.Core;
using Doitsu.Service.Core.Extensions;
using Doitsu.Service.Core.Services.EmailService;
using Microsoft.AspNetCore.Builder;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.DeliveryIntegration;
using Doitsu.Service.Core.Interfaces;

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
    public static class DoitsuEcommerceCoreConfiguration
    {
        private readonly static CultureInfo[] supportedCultures = {
            new CultureInfo("en-US"),
            new CultureInfo("vi-VN")
        };

        private readonly static RequestLocalizationOptions LocalizationOptions = new RequestLocalizationOptions()
        {
            DefaultRequestCulture = new RequestCulture("vi-VN"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures,
            RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider { Options = LocalizationOptions },
                new CookieRequestCultureProvider { Options = LocalizationOptions },
                new AcceptLanguageHeaderRequestCultureProvider { Options = LocalizationOptions }
            }
        };


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void UseDoitsuEcommerceCoreHosting(this IApplicationBuilder app, bool isConfigImageSharpWeb = false)
        {
            // Using authorize
            app.UseAuthentication();
            app.UseCookiePolicy();

            // Using resource files
            app.UseResponseCompression();
            if (isConfigImageSharpWeb)
                app.UseImageSharp();

            // Using localization
            app.UseRequestLocalization(LocalizationOptions);
        }

        public static IServiceCollection AddDoitsuEcommerceCore(this IServiceCollection services, IConfiguration configuration, IDatabaseConfigurer databaseConfigurer, bool isConfigImageSharpWeb = false)
        {
            #region Identity Database Config
            var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
            // Config identity db config
            services.AddDbContext<EcommerceDbContext>(builder =>
                    databaseConfigurer.Configure(builder, typeof(EcommerceDbContext).Assembly.GetName().Name),
                    ServiceLifetime.Scoped)
                .AddIdentity<EcommerceIdentityUser, EcommerceIdentityRole>()
                .AddEntityFrameworkStores<EcommerceDbContext>()
                .AddDefaultTokenProviders();
                
            services.RegisterDefaultEntityChangesHandlers();
            // Inject Identity Manager
            services.AddScoped(typeof(EcommerceIdentityUserManager<EcommerceIdentityUser>));
            services.AddScoped(typeof(EcommerceRoleIntManager<EcommerceIdentityRole>));
            services.AddScoped(typeof(EcommerceUserSignInManager<EcommerceIdentityUser>));
            services.AddScoped(typeof(IUserClaimsPrincipalFactory<EcommerceIdentityUser>), typeof(DoitsuCookieIdenittyCustomClaimsFactory<EcommerceIdentityUser>));

            // Authentication configure
            services.AddDoitsuBasicAuthorize();
            #endregion

            #region Config service
            services.AddDoitsuEcommerceCoreServices();
            services.ConfigDeliveryIntegration(configuration);
            #endregion

            #region Mapper Config
            services.AddDoitsuEcommerceCoreAutoMapper();
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

            #region Localization
            services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = LocalizationOptions.DefaultRequestCulture;
                options.SupportedCultures = LocalizationOptions.SupportedCultures;
                options.SupportedUICultures = LocalizationOptions.SupportedUICultures;
            });
            #endregion
            return services;
        }

        #region Config Image Sharp Methods
        private static void ConfigureCustomServicesAndCustomOptions(IServiceCollection services)
        {
            // Use the factory methods to configure the PhysicalFileSystemCacheOptions
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
                .SetMemoryAllocator(provider => ArrayPoolMemoryAllocator.CreateWithMinimalPooling())
                .Configure<PhysicalFileSystemCacheOptions>(options =>
                {
                    options.CacheFolder = "different-cache";
                })
                .SetCache<PhysicalFileSystemCache>()
                .SetCacheHash<CacheHash>()
                .AddProvider<PhysicalFileSystemProvider>()
                .AddProcessor<ResizeWebProcessor>()
                .AddProcessor<FormatWebProcessor>()
                .AddProcessor<BackgroundColorWebProcessor>();
        }
        #endregion

    }
}