using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ISliderService : IBaseService<Sliders>
    {
    }

    public class SliderService : BaseService<Sliders>, ISliderService
    {
        public SliderService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<Sliders, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
