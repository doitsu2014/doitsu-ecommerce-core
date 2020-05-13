using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using EFCore.Abstractions.EntityConfigurations;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class EcommerceIdentityUserConfiguration : BaseConfiguration<EcommerceIdentityUser>
    {
        public override Expression<Func<EcommerceIdentityUser, object>> KeyExpression => x => x.Id;
    }
}