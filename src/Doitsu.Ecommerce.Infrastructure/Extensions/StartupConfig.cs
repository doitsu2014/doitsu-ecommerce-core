using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data.Handlers;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.RazorPage;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services;
using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;
using Doitsu.Ecommerce.Infrastructure.AutoMapperProfiles;
using Doitsu.Ecommerce.Infrastructure.Data;
using Doitsu.Ecommerce.Infrastructure.Data.EntityChangeHandlers;
using Doitsu.Ecommerce.Infrastructure.IdentityServer4.Data;
using Doitsu.Ecommerce.Infrastructure.Repositories;
using Doitsu.Ecommerce.Infrastructure.Services.SmtpEmailServerHandler;
using Doitsu.Ecommerce.Infrastructure.Services.RazorPageRenderer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;

namespace Doitsu.Ecommerce.Infrastructure.Extensions
{
    public static partial class StartupConfig
    {
        public static IServiceCollection AddDoitsuEcommerceInfrastructure(this IServiceCollection services, IConfiguration configuration, IDatabaseConfigurer databaseConfigurer, bool isConfigImageSharpWeb = false, bool isUsingIdentityServer = false)
        {
            // Config identity db config
            services.AddDbContext<EcommerceDbContext>(builder =>
                    databaseConfigurer.Configure(builder, typeof(EcommerceDbContext).Assembly.GetName().Name),
                    ServiceLifetime.Scoped)
                .AddIdentity<EcommerceIdentityUser, EcommerceIdentityRole>()
                .AddEntityFrameworkStores<EcommerceDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped(typeof(IEcommerceDatabaseManager), typeof(EcommerceDbManager));

            // Entity Change Handlers
            services.AddScoped(typeof(IEntityChangeHandler), typeof(ActivableHandler));
            services.AddScoped(typeof(IEntityChangeHandler), typeof(VersWorkaroundHandler));
            services.AddScoped(typeof(IEntityChangeHandler), typeof(AuditableHandler));

            // Repositories
            services.AddScoped(typeof(IBaseEcommerceRepository<>), typeof(BaseEcommerceRepository<>));

            // Authentication configure
            if (isUsingIdentityServer)
            {
                services.AddEcommerceIs4Server(databaseConfigurer, typeof(EcommerceIs4ConfigurationDbContext).Assembly.GetName().Name, configuration);
                services.AddEcommerceIs4Authentication(configuration);
            }
            else
                services.AddDoitsuBasicAuthorize();

            if (isConfigImageSharpWeb) 
                services.AddDoitsuEcommerceImageSharpWeb();

            #region Services
            services.AddScoped<IRazorPageRenderer, RazorPageRenderer>();
            services.AddDoitsuEmailService(configuration);
            services.AddDeliveryIntegration(configuration);
            #endregion

            services.AddGzip();

            #region Localization
            services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = LocalizationOptions.DefaultRequestCulture;
                options.SupportedCultures = LocalizationOptions.SupportedCultures;
                options.SupportedUICultures = LocalizationOptions.SupportedUICultures;
            });
            #endregion

            #region AutoMapper
            services.AddAutoMapper(cfg => cfg.AddCollectionMappers(), typeof(DeliveryIntegrationMapperProfile).Assembly);
            #endregion

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

            return services;
        }

        internal static IServiceCollection AddGzip(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = new[]
                {
                    // Default
                    "text/plain",
                    "text/css",
                    "application/javascript",
                    "text/html",
                    "application/xml",
                    "text/xml",
                    "application/json",
                    "text/json",
                    // Custom
                    "image/svg+xml",
                    "image/jpg",
                    "image/jpeg",
                    "image/png",
                    "font/woff2",
                    "application/font-woff",
                    "application/font-ttf",
                    "application/font-eot",
                };
                options.EnableForHttps = true;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            return services;
        }

        internal static IServiceCollection AddDoitsuEcommerceImageSharpWeb(this IServiceCollection services)
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

            return services;
        }

        /// <summary>
        /// Add Scoped ISmtpEmailServerHandler to handle smtp service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddDoitsuEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpMailServerOptions>(configuration.GetSection("SmtpMailServerOptions"));
            services.Configure<LeaderMail>(configuration.GetSection("LeaderEmail"));
            services.AddTransient(typeof(ISmtpEmailServerHandler), typeof(SmtpEmailServerHandler));
            return services;
        }

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
        public static void UseDoitsuEcommerceCoreHosting(this IApplicationBuilder app, IWebHostEnvironment env, bool isConfigImageSharpWeb = false, bool isUsingIdentityServer = false)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.Use((context, next) =>
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-Proto", out StringValues proto))
                {
                    context.Request.Protocol = proto;
                    context.Request.Scheme = proto;
                }
                return next();
            });
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
