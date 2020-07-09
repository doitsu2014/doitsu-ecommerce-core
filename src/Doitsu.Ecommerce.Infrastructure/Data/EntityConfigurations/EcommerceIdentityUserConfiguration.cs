using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;


namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class EcommerceIdentityUserConfiguration : BaseConfiguration<EcommerceIdentityUser>
    {
        public override Expression<Func<EcommerceIdentityUser, object>> KeyExpression => x => x.Id;
    }
}