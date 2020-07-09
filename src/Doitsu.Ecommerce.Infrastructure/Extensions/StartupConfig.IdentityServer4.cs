using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.AuthorizeBuilder;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Models.IdentityServer4;
using Doitsu.Ecommerce.Infrastructure.IdentityServer4.Data;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static Doitsu.Ecommerce.ApplicationCore.Constants;

namespace Doitsu.Ecommerce.Infrastructure.Extensions
{
    public static partial class StartupConfig
    {
        public static IServiceCollection AddEcommerceIs4Server(this IServiceCollection services, IDatabaseConfigurer databaseConfigurer, string migrationAssembly, IConfiguration configuration)
        {
            var isc = configuration.GetSection(nameof(IdentityServerConfiguration)).Get<IdentityServerConfiguration>();

            IIdentityServerBuilder AddAndGetIdentityServerBuilder()
            {
                return services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.UserInteraction.LoginUrl = "/nguoi-dung/dang-nhap";
                    options.UserInteraction.LogoutUrl = "/nguoi-dung/dang-xuat";

                    // options.Discovery.CustomEntries.Add("identity_endpoint", "~/api/identity");
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
            }

            X509Certificate2 GetX509Certificate2()
            {
                if (!File.Exists(isc.CertificatePath))
                    throw new InvalidOperationException($"No certificate by name '{isc.CertificatePath}'");
                return new X509Certificate2(isc.CertificatePath, isc.CertificatePassword, X509KeyStorageFlags.MachineKeySet);
            }
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

            return services;
        }

        public static IServiceCollection AddEcommerceIs4Authentication(this IServiceCollection services, IConfiguration configuration)
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

    }
}
