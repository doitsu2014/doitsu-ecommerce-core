﻿using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Doitsu.Ecommerce.ApplicationCore.Services.IdentityManagers
{
    public class EcommerceRoleIntManager<T> : RoleManager<T>
        where T : EcommerceIdentityRole
    {
        public EcommerceRoleIntManager(IRoleStore<T> store, IEnumerable<IRoleValidator<T>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<EcommerceRoleIntManager<T>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {

        }
    }
}