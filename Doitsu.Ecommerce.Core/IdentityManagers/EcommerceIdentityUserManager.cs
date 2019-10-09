using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.AuthorizeBuilder;
using Doitsu.Ecommerce.Core.Data.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Doitsu.Ecommerce.Core.IdentitiesExtension
{
    public class EcommerceIdentityUserManager<T> : UserManager<T>
        where T : EcommerceIdentityUser
    {

        public EcommerceIdentityUserManager(IUserStore<T> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<T> passwordHasher, IEnumerable<IUserValidator<T>> userValidators, IEnumerable<IPasswordValidator<T>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<T>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        /// <summary>
        /// As no tracking and find use by phone number
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<EcommerceIdentityUser> FindByPhoneAsNoTrackingAsync(string phone)
        {
            var user = await this.Users.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            return user;
        }

        public async Task<TokenAuthorizeModel> GetJwtAuthorizeModelAsync(T user, int expireDays = 7)
        {
            var userRoles = (await this.GetRolesAsync(user));

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                };

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.Default.GetBytes(DoitsuJWTValidators.SecretKey);
            var issuer = DoitsuJWTValidators.Issuer;
            var audience = DoitsuJWTValidators.Audience;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.ToVietnamDateTime().AddDays(expireDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return new TokenAuthorizeModel
            {
                Token = tokenString,
                ValidTo = token.ValidTo,
                ValidFrom = token.ValidFrom
            };
        }
    }
}