using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.AuthorizeBuilder;
using Doitsu.Service.Core.Services.EmailService;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using static Doitsu.Ecommerce.Core.Abstraction.Constants;

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
                            ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
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
                options.MimeTypes = new []
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