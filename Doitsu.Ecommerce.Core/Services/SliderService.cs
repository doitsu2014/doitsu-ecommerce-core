using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
namespace Doitsu.Ecommerce.Core.Services
{
    public interface ISliderService : IBaseService<Sliders>
    {
    }

    public class SliderService : BaseService<Sliders>, ISliderService
    {
        public SliderService(IEcommerceUnitOfWork unitOfWork, ILogger<BaseService<Sliders>> logger) : base(unitOfWork, logger)
        {
        }
    }
}
