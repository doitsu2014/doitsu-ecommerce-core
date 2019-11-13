using Doitsu.Ecommerce.Core.Tests.Helpers;
using Xunit.Abstractions;
namespace Doitsu.Ecommerce.Core.Tests
{
    public class ProductServiceTest : BaseServiceTest<ProductServiceFixture>
    {
        public ProductServiceTest(ProductServiceFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
        }
        // private async Task<IIs4Service> GetIdentityServiceAndInitializeDb(IWebHost host)
        // {
        //     var identityServerService = host.Services.GetService<IIs4Service>();

        //     var systemService = host.Services.GetService<ISystemService>();
        //     await systemService.MigrateAsync();

        //     return identityServerService;
        // }
    }
}
