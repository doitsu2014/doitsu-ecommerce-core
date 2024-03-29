using System.Reflection.Metadata;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Abstraction.Configuration;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Ecommerce.Core.AuthorizeBuilder;
using Doitsu.Ecommerce.Core.IdentityServer4.Data;
using Doitsu.Service.Core.Interfaces;
using Doitsu.Service.Core.Services.EmailService;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using static Doitsu.Ecommerce.Core.Abstraction.Constants;
using Doitsu.Ecommerce.Core.Abstraction;
using IdentityServer4;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Configuration;

namespace Doitsu.Service.Core.Extensions
{
    public static class DoitsuServiceExtension
    {
        /// <summary>
        /// Add Scoped ISmtpEmailServerHandler to handle smtp service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDoitsuEmailService(this IServiceCollection services)
        {
            services.AddScoped(typeof(ISmtpEmailServerHandler), typeof(SmtpEmailServerHandler));
            return services;
        }

        public static IServiceCollection AddEcommerceIs4Server(this IServiceCollection services, IDatabaseConfigurer databaseConfigurer, string migrationAssembly, IConfiguration configuration)
        {
            var isc = configuration.GetSection(nameof(IdentityServerConfiguration)).Get<IdentityServerConfiguration>();

            Func<IIdentityServerBuilder> AddAndGetIdentityServerBuilder = () => 
            {
                return services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    // options.Discovery.CustomEntries.Add("identity_endpoint", "~/api/identity");
                    options.UserInteraction.LoginUrl = "/nguoi-dung/dang-nhap";
                    options.UserInteraction.LogoutUrl = "/nguoi-dung/dang-xuat";
                    // options.UserInteraction.ConsentUrl = "/api/is4/consent";
                    // options.UserInteraction.ErrorUrl = "/api/is4/error";
                    // options.UserInteraction.DeviceVerificationUrl = "/api/is4/device-verification";
                })
                .AddConfigurationStore<EcommerceIs4ConfigurationDbContext>(options =>
                {
                    options.ConfigureDbContext = builder => databaseConfigurer.Configure(builder, migrationAssembly);
                })
                .AddOperationalStore<EcommerceIs4PersistedGrantDbContext>(options =>
                {
                    options.ConfigureDbContext = builder => databaseConfigurer.Configure(builder, migrationAssembly);
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600;
                })
                .AddAspNetIdentity<EcommerceIdentityUser>();
            };

            Func<X509Certificate2> GetX509Certificate2 = () =>
            {
                if (!File.Exists(isc.CertificatePath))
                    throw new InvalidOperationException($"No certificate by name '{isc.CertificatePath}'");
                return new X509Certificate2(isc.CertificatePath, isc.CertificatePassword, X509KeyStorageFlags.MachineKeySet);
            };
            // services.AddTransient(typeof(ICache<>), typeof(IdentityServers4Cache<>));

            services.AddDbContext<ConfigurationDbContext>(builder =>
                    databaseConfigurer.Configure(builder, typeof(EcommerceIs4ConfigurationDbContext).Assembly.GetName().Name),
                    ServiceLifetime.Scoped);

            services.AddDbContext<PersistedGrantDbContext>(builder =>
                    databaseConfigurer.Configure(builder, typeof(EcommerceIs4PersistedGrantDbContext).Assembly.GetName().Name),
                    ServiceLifetime.Scoped);

            IdentityModelEventSource.ShowPII = true; //To show detail of error and see the problem

            var isBuilder = isc.IsProduction 
                ? AddAndGetIdentityServerBuilder().AddSigningCredential(GetX509Certificate2()) 
                : AddAndGetIdentityServerBuilder().AddDeveloperSigningCredential();

            services.AddEcommerceIs4Authentication(configuration);
            return services;
        }

        private static IServiceCollection AddEcommerceIs4Authentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var mvcFeAppConfiguration = configuration.GetSection(nameof(MvcFrontEndAppConfiguration)).Get<MvcFrontEndAppConfiguration>();
            services
                .AddAuthentication(opts =>
                {
                    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString(mvcFeAppConfiguration.CookieLoginPath.IsNullOrEmpty() ? "/nguoi-dung/dang-nhap" : mvcFeAppConfiguration.CookieLoginPath);
                    options.LogoutPath = new PathString(mvcFeAppConfiguration.CookieLogoutPath.IsNullOrEmpty() ? "/nguoi-dung/dang-xuat" : mvcFeAppConfiguration.CookieLogoutPath);
                    options.Events.OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult(0);
                    };
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = mvcFeAppConfiguration.AuthorityUrl;
                    options.RequireHttpsMetadata = mvcFeAppConfiguration.OidcRequireHttpsMetadata;
                    options.ClientId = mvcFeAppConfiguration.ClientId;
                    options.ClientSecret = mvcFeAppConfiguration.ClientSecret;
                    options.ResponseType = "code id_token";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Scope.Add(Constants.EcommerceIs4Scopes.USER);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                })
                .AddJwtBearer(DoitsuAuthenticationSchemes.JWT_SCHEME, options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = DoitsuJWTValidators.Audience,
                        ValidIssuer = DoitsuJWTValidators.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(DoitsuJWTValidators.SecretKey))
                    };
                });

            return services;
        }

        /// <summary>
        /// Add Scoped basic authorization methods, with the default authenticated schemes of Cookies (CookieAuthenticationDefaults.AuthenticationScheme)
        /// And api scheme is your choice
        /// The first is Add Cookie
        /// The second is JwtBearer
        /// </summary>
        /// <param name="services"></param>
        /// <param name="apiSegment">api segment default is /api, that is config to help the system know what url segment as a Api Endpoint</param>
        /// <param name="defaultLoginPath">default is /nguoi-dung/dang-nhap</param>
        /// <param name="defaultLogoutPath">/nguoi-dung/dang-xuat</param>
        /// <returns></returns>
        public static IServiceCollection AddDoitsuBasicAuthorize(this IServiceCollection services,
            string jwtBearerScheme = DoitsuAuthenticationSchemes.JWT_SCHEME,
            string jwtBearerSecret = DoitsuJWTValidators.DEFAULT_SECRET_KEY,
            string apiSegment = "/api",
            string defaultLoginPath = "/nguoi-dung/dang-nhap",
            string defaultLogoutPath = "/nguoi-dung/dang-xuat")
        {
            // Authentication configure
            services
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.LoginPath = new PathString(defaultLoginPath);
                    options.LogoutPath = new PathString(defaultLogoutPath);
                    options.Events.OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments(apiSegment))
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult(0);
                    };
                })
                .AddJwtBearer(jwtBearerScheme, options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = DoitsuJWTValidators.Audience,
                        ValidIssuer = DoitsuJWTValidators.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(DoitsuJWTValidators.SecretKey))
                    };
                });
            return services;
        }


        public static IServiceCollection AddGzip(this IServiceCollection services)
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

    }
}