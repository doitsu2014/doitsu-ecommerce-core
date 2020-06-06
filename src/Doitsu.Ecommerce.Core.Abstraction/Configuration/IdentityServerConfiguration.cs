namespace Doitsu.Ecommerce.Core.Abstraction.Configuration
{
    public class MvcFrontEndAppConfiguration
    {
        public string AuthorityUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool OidcRequireHttpsMetadata { get; set; }
        public string CookieLoginPath { get; set; }
        public string CookieLogoutPath { get; set; }
    }

    public class SpaAdminAppConfiguration {
        public string AuthorityUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}