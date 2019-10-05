using Microsoft.AspNetCore.Authentication.Cookies;

namespace Doitsu.Ecommerce.Core
{
    public class Utils
    {
        public class AppConstants
        {
            public const string API_JWT_COOKIE = CookieAuthenticationDefaults.AuthenticationScheme + "," + API_JWT_AUTH_SCHEME;
            public const string API_JWT_AUTH_SCHEME = "api_jwt";
            public const string ADMIN_AUTH_SCHEME = "admin";
        }

    }
}