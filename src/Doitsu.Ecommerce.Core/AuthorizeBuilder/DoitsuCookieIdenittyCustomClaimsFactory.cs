﻿using System.Security.Claims;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Ecommerce.Core.IdentitiesExtension;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Doitsu.Ecommerce.Core.AuthorizeBuilder
{
    public class DoitsuCookieIdenittyCustomClaimsFactory<TDoitsuUserInt> : UserClaimsPrincipalFactory<TDoitsuUserInt, EcommerceIdentityRole>
        where TDoitsuUserInt : EcommerceIdentityUser
        {
            public DoitsuCookieIdenittyCustomClaimsFactory(EcommerceIdentityUserManager<TDoitsuUserInt> userManager, RoleManager<EcommerceIdentityRole> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options) {}

            public async override Task<ClaimsPrincipal> CreateAsync(TDoitsuUserInt user)
            {
                var principal = await base.CreateAsync(user);
                ((ClaimsIdentity)principal.Identity).AddClaims(new []
                {
                    new Claim(ClaimTypes.GivenName, user.Fullname)
                });
                return principal;
            }
        }

}