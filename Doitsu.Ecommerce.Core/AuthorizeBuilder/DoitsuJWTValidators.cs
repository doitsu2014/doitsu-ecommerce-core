using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.Core.AuthorizeBuilder
{
    /// <summary>
    /// This class provide Constant Validators
    /// </summary>
    public static class DoitsuJWTValidators
    {
        public static string Issuer { get { return "http://doitsu.tech"; } }
        public static string Audience { get { return "http://doitsu.tech"; } }
        public static string CookieName { get { return "DoitsuTokenCookie"; } }
        public static string SecretKey { get { return "@everone:DoitsuSecret!"; } }
        public const string DEFAULT_SECRET_KEY = "@everone:DoitsuSecret!";
        public const string DEFAULT_ISSUER = "http://doitsu.tech";
        public const string DEFAULT_AUDIENCE = "http://doitsu.tech";
    }
}
