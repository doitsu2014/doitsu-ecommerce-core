using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ISliderService : IEcommerceBaseService<Sliders>
    {
    }

    public class SliderService : EcommerceBaseService<Sliders>, ISliderService
    {
        public SliderService(EcommerceDbContext dbContext, IMapper mapper, ILogger<EcommerceBaseService<Sliders>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
