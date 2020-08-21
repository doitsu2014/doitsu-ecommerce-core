using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using TestSupport.Attributes;
using Xunit.Abstractions;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public abstract class BaseServiceTest<TFixture>
        where TFixture : class
    {
        protected readonly TFixture _fixture;
        protected readonly ITestOutputHelper _testOutputHelper;

        public BaseServiceTest(TFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _testOutputHelper = testOutputHelper;
        }
    }
}
