using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Identities;

namespace Doitsu.Ecommerce.Core.Services
{
    /// <summary>
    /// This service is a pure service for AspNetUser
    /// </summary>
    public interface IAspNetUserService : IBaseService<EcommerceIdentityUser>
    {

    }

    public class AspNetUserService : BaseService<EcommerceIdentityUser>, IAspNetUserService
    {
        public AspNetUserService(IUnitOfWork unitOfWork, ILogger<BaseService<EcommerceIdentityUser>> logger) : base(unitOfWork, logger)
        {

        }
    }
}
