namespace Doitsu.Ecommerce.Core.Abstraction.Configuration
{
    public class IdentityServerConfiguration
    {
        public string ServerUrl { get; set; }
        public string SystemName { get; set; }
        public string GodClientId { get; set; }
        public string GodClientSecret { get; set; }
    }
}