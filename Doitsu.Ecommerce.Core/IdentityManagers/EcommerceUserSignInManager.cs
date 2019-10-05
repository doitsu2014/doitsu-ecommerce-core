using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Data.Identities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Doitsu.Ecommerce.Core.IdentitiesExtension
{
    public class EcommerceUserSignInManager<T> : SignInManager<T>
        where T : EcommerceIdentityUser
    {
        public EcommerceUserSignInManager(UserManager<T> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<T> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<T>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<T> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public async Task SignInCookieAsync(T user, string password, int expireMinutes = 30)
        {
            var result = await this.UserManager.CheckPasswordAsync(user, password);
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.PhoneNumber),
                    new Claim("Fullname", user.Fullname)
                };

            var userRoles = await UserManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(expireMinutes)
            };

            await Context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}