using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ISliderService : IBaseService<Sliders>
    {
    }

    public class SliderService : BaseService<Sliders>, ISliderService
    {
        public SliderService(IUnitOfWork unitOfWork, ILogger<BaseService<Sliders>> logger) : base(unitOfWork, logger)
        {
        }
    }
}
