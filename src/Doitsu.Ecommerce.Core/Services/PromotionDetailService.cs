using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IPromotionDetailService : IBaseService<PromotionDetail>
    {
    }

    public class PromotionDetailService : BaseService<PromotionDetail>, IPromotionDetailService
    {
        public PromotionDetailService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<PromotionDetail, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
