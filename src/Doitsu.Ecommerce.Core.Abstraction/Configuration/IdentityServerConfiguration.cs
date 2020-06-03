namespace Doitsu.Ecommerce.Core.Abstraction.Configuration
{
    public class IdentityServerConfiguration
    {
        public string AuthorityUrl { get; set; }
        public string SystemName { get; set; }
        public string MvcFrontEndAppClientId { get; set; }
        public string MvcFrontEndAppClientSecret { get; set; }
        public string SpaAdminAppClientId { get; set; }
        public string SpaAdminEndAppClientId { get; set; }
    }
}