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
using Doitsu.Ecommerce.Core.IdentityServer4.Data;
using Doitsu.Ecommerce.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc;

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

        public static IServiceCollection AddDoitsuEcommerceCore(this IServiceCollection services, IConfiguration configuration, IDatabaseConfigurer databaseConfigurer, bool isConfigImageSharpWeb = false, bool isUsingIdentityServer = false)
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
            if (isUsingIdentityServer)
            {
                services.AddEcommerceIs4Server(databaseConfigurer, typeof(EcommerceIs4ConfigurationDbContext).Assembly.GetName().Name, configuration);
            }
            else
            {
                services.AddDoitsuBasicAuthorize();
            }
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
            
            
            #region Last
            services.ConfigDeliveryIntegration(configuration);
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.AddControllersWithViews(opts =>
                {
                    opts.MaxModelValidationErrors = 50;
                    opts.EnableEndpointRouting = false;
                })
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void UseDoitsuEcommerceCoreHosting(this IApplicationBuilder app, IWebHostEnvironment env, bool isConfigImageSharpWeb = false, bool isUsingIdentityServer = false)
        {
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            // Using authorize
            if (isUsingIdentityServer)
            {
                app.UseIdentityServer();
            }
            else
            {
                app.UseAuthentication();
            }

            // Using resource files
            app.UseResponseCompression();
            if (isConfigImageSharpWeb)
                app.UseImageSharp();

            // Using localization
            app.UseRequestLocalization(LocalizationOptions);

            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // app.UseMvc(routes =>
            // {
            //     routes.MapRoute(
            //         "default",
            //         "{controller=Home}/{action=Index}/{id?}");

            //     routes.MapSpaFallbackRoute(
            //         name: "spa-fallback-admin",
            //         defaults: new
            //         {
            //             controller = "Admin",
            //             action = "Index"
            //         }
            //     );
            // });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}