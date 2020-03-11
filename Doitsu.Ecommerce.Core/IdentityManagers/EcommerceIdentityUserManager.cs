using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.AuthorizeBuilder;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Optional;
using Optional.Async;
using static Doitsu.Ecommerce.Core.Constants;

namespace Doitsu.Ecommerce.Core.IdentitiesExtension
{
    public class EcommerceIdentityUserManager<T> : UserManager<T>
        where T : EcommerceIdentityUser
    {
        public EcommerceIdentityUserManager(IUserStore<T> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<T> passwordHasher, IEnumerable<IUserValidator<T>> userValidators, IEnumerable<IPasswordValidator<T>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<T>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<bool> AnyEmailAsync(string email)
        {
            if (email.IsNullOrEmpty()) return false;
            email = email.ToLower().Trim();
            return await this.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> AnyPhoneNumberAsync(string phoneNumber)
        {
            if (phoneNumber.IsNullOrEmpty()) return false;
            phoneNumber = phoneNumber.ToLower().Trim();
            return await this.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// As no tracking and find use by phone number
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<T> FindByPhoneAsNoTrackingAsync(string phone)
        {
            var user = await this.Users.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            return user;
        }

        /// <summary>
        /// As no tracking and find use by phone number
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<T> FindByIdentityInformationAsync(string identityInformation)
        {
            identityInformation = identityInformation?.Trim().ToLower();
            var user = await this.Users.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == identityInformation || x.Email == identityInformation);
            return user;
        }

        public async Task<Option<T, string>> FindUserByEmailAndPassword(LoginByEmailViewModel loginData)
        {
            return await loginData.SomeNotNull()
                .WithException("Dữ liệu login không được để trống")
                .Filter(d => !d.Email.IsNullOrEmpty(), "Địa chỉ mail không được để trống.")
                .Filter(d => !d.Password.IsNullOrEmpty(), "Mật khẩu không được để trống.")
                .FlatMapAsync(async d =>
                {
                    var user = await this.FindByEmailAsync(d.Email);
                    if (user == null)
                    {
                        return Option.None<T, string>("Không tìm thấy địa chỉ email của bạn trông hệ thống.");
                    }
                    else
                    {
                        var truePassword = await this.CheckPasswordAsync(user, d.Password);
                        if (!truePassword)
                        {
                            return Option.None<T, string>("Email và mật khẩu không hợp lệ.");
                        }
                    }
                    return Option.Some<T, string>(user);
                });
        }

        public async Task<Option<T, string>> FindUserByPhoneAndPasswordAsync(LoginByPhoneViewModel loginData)
        {
            return await loginData.SomeNotNull()
                .WithException("Dữ liệu login không được để trống")
                .Filter(d => !d.PhoneNumber.IsNullOrEmpty(), "Số điện thoại không được để trống.")
                .Filter(d => !d.Password.IsNullOrEmpty(), "Mật khẩu không được để trống.")
                .FlatMapAsync(async d =>
                {
                    var user = await this.FindByPhoneAsNoTrackingAsync(d.PhoneNumber);
                    if (user == null)
                    {
                        return Option.None<T, string>("Không tìm thấy số điện thoại của bạn trông hệ thống.");
                    }
                    else
                    {
                        var truePassword = await this.CheckPasswordAsync(user, d.Password);
                        if (!truePassword)
                        {
                            return Option.None<T, string>("Số điện thoại và mật khẩu không hợp lệ.");
                        }
                    }
                    return Option.Some<T, string>(user);
                });
        }

        public async Task<ClaimsIdentity> CreateClaimIdentityAsync(T user, int expireDays = 7, string authenticationType = CookieAuthenticationDefaults.AuthenticationScheme)
        {
            var userRoles = (await this.GetRolesAsync(user));
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("Fullname", user.Fullname),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(Constants.ClaimTypeConstants.USER_ID, user.Id.ToString())
                };


            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return new ClaimsIdentity(claims, authenticationType);
        }

        public async Task<TokenAuthorizeModel> GetJwtAuthorizeModelAsync(T user, int expireDays = 7)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.Default.GetBytes(DoitsuJWTValidators.SecretKey);
            var issuer = DoitsuJWTValidators.Issuer;
            var audience = DoitsuJWTValidators.Audience;
            var claimIdentity = await CreateClaimIdentityAsync(user, expireDays, DoitsuAuthenticationSchemes.JWT_SCHEME);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = claimIdentity,
                Expires = DateTime.UtcNow.ToVietnamDateTime().AddDays(expireDays),
                SigningCredentials = signingCredentials
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