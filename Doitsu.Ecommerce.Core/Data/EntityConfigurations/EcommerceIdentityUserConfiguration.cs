using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.Core.Data.Identities;
using EFCore.Abstractions.EntityConfigurations;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class EcommerceIdentityUserConfiguration : BaseConfiguration<EcommerceIdentityUser>
    {
        public override Expression<Func<EcommerceIdentityUser, object>> KeyExpression => x => x.Id;
    }
}