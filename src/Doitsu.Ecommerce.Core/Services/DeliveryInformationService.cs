using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using System.Collections.Generic;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IDeliveryInformationService : IEcommerceBaseService<DeliveryInformation>
    {
    }

    public class DeliveryInformationService : EcommerceBaseService<DeliveryInformation>, IDeliveryInformationService 
    {
        public DeliveryInformationService(EcommerceDbContext dbContext,
                          IMapper mapper,
                          ILogger<EcommerceBaseService<DeliveryInformation>> logger) : base(dbContext, mapper, logger)
        {
        }

    }
}
