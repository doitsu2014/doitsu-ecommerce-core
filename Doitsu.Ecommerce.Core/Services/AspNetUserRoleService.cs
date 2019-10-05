using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Identities;

namespace Doitsu.Ecommerce.Core.Services
{
    /// <summary>
    /// This service is a pure service for AspNetUserRole
    /// </summary>
    public interface IAspNetUserRoleService : IBaseService<EcommerceIdentityRole>
    {

    }

    public class AspNetUserRoleService : BaseService<EcommerceIdentityRole>, IAspNetUserRoleService
    {
        public AspNetUserRoleService(IUnitOfWork unitOfWork, ILogger<BaseService<EcommerceIdentityRole>> logger) : base(unitOfWork, logger)
        {

        }
    }
}
