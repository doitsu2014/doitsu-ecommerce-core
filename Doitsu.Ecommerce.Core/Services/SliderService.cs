using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;

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
