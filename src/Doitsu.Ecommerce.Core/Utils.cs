using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Doitsu.Ecommerce.Core
{
    public class Utils
    {
        public class AppConstants
        {
            public const string API_JWT_COOKIE_OIDC = CookieAuthenticationDefaults.AuthenticationScheme + "," + API_JWT_AUTH_SCHEME + "," + OpenIdConnectDefaults.AuthenticationScheme;
            public const string API_JWT_COOKIE = CookieAuthenticationDefaults.AuthenticationScheme + "," + API_JWT_AUTH_SCHEME;
            public const string API_JWT_AUTH_SCHEME = "api_jwt";
            public const string ADMIN_AUTH_SCHEME = "admin";
        }
    }
}