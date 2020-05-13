using Microsoft.Extensions.Localization;

namespace Doitsu.Ecommerce.Core
{
    public interface IGeneralResource
    {
    }
    /// <summary>
    /// Dummy class for general resource resolving
    /// </summary>
    public class GeneralResource : IGeneralResource
    {
        private readonly IStringLocalizer _localizer;

        public GeneralResource(IStringLocalizer<GeneralResource> localizer)
        {
            _localizer = localizer;
        }

        public string this[string index]
        {
            get
            {
                return _localizer[index];
            }
        }
    }
}