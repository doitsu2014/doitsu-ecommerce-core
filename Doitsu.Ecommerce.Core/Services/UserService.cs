using System.Security.Principal;
using System.Linq;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.IdentitiesExtension;
using Doitsu.Ecommerce.Core.IdentityManagers;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Doitsu.Ecommerce.Core.ViewModels;
using AutoMapper;
using Optional;
using Optional.Async;
using static Doitsu.Ecommerce.Core.Constants;
using Doitsu.Ecommerce.Core.AuthorizeBuilder;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Doitsu.Service.Core.Services.EmailService;
using AutoMapper.QueryableExtensions;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{

    public interface IUserService
    {
        Task<ImmutableList<EcommerceIdentityUserViewModel>> GetUsersAsync(string name = "", string phone = "", int id = 0);
        Task<ImmutableList<EcommerceIdentityUserViewModel>> GetAllUsersAsync();
        Task<UserInforViewModel> GetUserByIdAsync(string userId);
        Task<ImmutableList<EcommerceIdentityRoleViewModel>> GetAllRolesAsync();
        Task<Option<TokenAuthorizeModel, string>> LoginByAdminAsync(AdminLoginViewModel request);
        Task<Option<EcommerceIdentityUser, string>> FindUserByEmailAndPassword(LoginByEmailViewModel loginData);
        Task<Option<EcommerceIdentityUser, string>> FindUserByPhoneAndPasswordAsync(LoginByPhoneViewModel loginData);
        Task<ClaimsIdentity> CreateClaimIdentityAsync(EcommerceIdentityUser user, int expireDays = 7, string authenticationType = CookieAuthenticationDefaults.AuthenticationScheme);
        Task<Option<ClaimsPrincipal, string>> RegisterCustomerAccountAsync(RegisterViewModel request);
        Task<Option<(ClaimsPrincipal cp, string code), string>> RegisterWithOrderAsync(RegisterViewModel request);
        Task<Option<string, string>> GetResettingPasswordAsync(string userIdentification, string scheme, string host);
        Task<Option<string, string>> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel);
        Task<Option<string, string>> ChangePasswordAsync(string userId, ChangePasswordViewMode request);
        Task<Option<string, string>> ChangeDetailsAsync(string userId, UpdateDetailViewModel request);
        Task<Option<EcommerceIdentityUser, string>> CreateNewUserAsync(AdminEditNewUserViewModel request);
        Task<Option<EcommerceIdentityUser, string>> UpdateUserAsync(AdminEditNewUserViewModel request);
    }

    public class UserService : IUserService
    {
        private readonly EcommerceRoleIntManager<EcommerceIdentityRole> roleService;
        private readonly EcommerceIdentityUserManager<EcommerceIdentityUser> userService;
        private readonly IOrderService orderService;
        private readonly IBrandService brandService;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;
        public UserService(EcommerceRoleIntManager<EcommerceIdentityRole> roleService,
                           EcommerceIdentityUserManager<EcommerceIdentityUser> userService,
                           IOrderService orderService,
                           IBrandService brandService,
                           IEmailService emailService,
                           IMapper mapper)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.orderService = orderService;
            this.brandService = brandService;
            this.emailService = emailService;
            this.mapper = mapper;
        }

        public async Task<ClaimsIdentity> CreateClaimIdentityAsync(EcommerceIdentityUser user, int expireDays = 7, string authenticationType = "Cookies")
        {
            return await userService.CreateClaimIdentityAsync(user);
        }

        public async Task<Option<EcommerceIdentityUser, string>> FindUserByEmailAndPassword(LoginByEmailViewModel loginData)
        {
            return await userService.FindUserByEmailAndPassword(loginData);
        }

        public async Task<Option<EcommerceIdentityUser, string>> FindUserByPhoneAndPasswordAsync(LoginByPhoneViewModel loginData)
        {
            return await userService.FindUserByPhoneAndPasswordAsync(loginData);
        }

        public async Task<UserInforViewModel> GetUserByIdAsync(string userId)
        {
            var user = await userService.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return mapper.Map<UserInforViewModel>(user);
        }
        public async Task<ImmutableList<EcommerceIdentityUserViewModel>> GetAllUsersAsync()
        {
            var userQuery = this.userService.Users.Where(u => u.Active).Include(u => u.UserRoles).AsNoTracking();

            return await GetUserViewModelAsync(userQuery);
        }

        public async Task<ImmutableList<EcommerceIdentityUserViewModel>> GetUsersAsync(string userName = "", string phoneNumber = "", int id = 0)
        {
            var userQuery = this.userService.Users.Where(u => u.Active).Include(u => u.UserRoles).AsNoTracking();
            
            var listFilterOfUser = new List<(bool filterable, Func<EcommerceIdentityUser, bool> function)>();
            listFilterOfUser.Add((!string.IsNullOrEmpty(userName), u => u.UserName.StartsWith(userName)));
            listFilterOfUser.Add((!string.IsNullOrEmpty(phoneNumber), u => u.PhoneNumber.StartsWith(phoneNumber)));
            listFilterOfUser.Add((id > 0, u => u.Id == id));
            listFilterOfUser.Where(exp => exp.filterable).ToList()
                .ForEach(exp => userQuery = userQuery.Where(exp.function).AsQueryable());

            return await GetUserViewModelAsync(userQuery);
        }

        private async Task<ImmutableList<EcommerceIdentityUserViewModel>> GetUserViewModelAsync(IQueryable<EcommerceIdentityUser> ecommerceIdentityUsers)
        {
            var roleQuery = (await this.roleService.Roles.AsNoTracking().ToListAsync()).ToImmutableList();
            
            return (ecommerceIdentityUsers.ToList())
           .Select(u =>
           {
               var userVm = this.mapper.Map<EcommerceIdentityUserViewModel>(u);
               userVm.EcommerceIdentityRoles = u.UserRoles.Select(ur =>
               {
                   var role = roleQuery.FirstOrDefault(r => r.Id == ur.RoleId);
                   return this.mapper.Map<EcommerceIdentityRoleViewModel>(role);
               }).ToList();
               return userVm;
           }).ToImmutableList();
        }

        public async Task<Option<TokenAuthorizeModel, string>> LoginByAdminAsync(AdminLoginViewModel request)
        {
            return await request.SomeNotNull()
                .WithException(UserMessage.LOGIN_NULL_REQUEST)
                .Filter(req => !string.IsNullOrWhiteSpace(req.Email), UserMessage.EMAIL_REQUIRED)
                .Filter(req => !string.IsNullOrWhiteSpace(req.Password), UserMessage.PASSWORD_REQUIRED)
                .FlatMapAsync(async req =>
                {
                    var user = await this.userService.FindByEmailAsync(req.Email);

                    if (user == null)
                    {
                        return Option.None<TokenAuthorizeModel, string>(UserMessage.LOGIN_FAILED);
                    }

                    var loginSuccess = await this.userService.CheckPasswordAsync(user, req.Password);

                    if (!loginSuccess)
                    {
                        return Option.None<TokenAuthorizeModel, string>(UserMessage.LOGIN_FAILED);
                    }

                    return Option.Some<TokenAuthorizeModel, string>(await this.userService.GetJwtAuthorizeModelAsync(user));
                });
        }

        public async Task<Option<ClaimsPrincipal, string>> RegisterCustomerAccountAsync(RegisterViewModel request)
        {
            return await request
                    .SomeNotNull()
                    .WithException(UserMessage.REGISTER_NULL_REQUEST)
                    .FlatMapAsync(async d =>
                    {
                        var user = mapper.Map<EcommerceIdentityUser>(d);
                        var claimsPrincipal = await RegisterCustomerAsync(user, d.Password);
                        return claimsPrincipal;
                    });
        }

        public async Task<Option<(ClaimsPrincipal cp, string code), string>> RegisterWithOrderAsync(RegisterViewModel request)
        {
            return (await request
                .SomeNotNull()
                .WithException(UserMessage.REGISTER_NULL_REQUEST)
                .Filter(req => req.CartInformation != null, UserMessage.CHECKOUT_CARD_NULL)
                .FlatMapAsync(async req =>
                {
                    var user = mapper.Map<EcommerceIdentityUser>(req);
                    var claimsPrincipal = await RegisterCustomerAsync(user, req.Password);

                    return await claimsPrincipal
                            .FlatMapAsync(async cp =>
                            {
                                var checkoutCard = await orderService.CheckoutCartAsync(req.CartInformation, user);
                                return checkoutCard
                                .FlatMap(cc =>
                                {
                                   return Option.Some<(ClaimsPrincipal cp, string code), string>((cp: cp, code: cc));
                                });
                            });
                }));
        }

        private async Task<Option<ClaimsPrincipal, string>> RegisterCustomerAsync(EcommerceIdentityUser user, string password)
        {
            return await new
            {
                user,
                password
            }.SomeNotNull().WithException(string.Empty)
                .Filter(d => !d.password.IsNullOrEmpty(), UserMessage.PASSWORD_REQUIRED)
                .Filter(d => !d.user.PhoneNumber.IsNullOrEmpty(), UserMessage.PHONE_REQUIRED)
                .Filter(d => !d.user.Fullname.IsNullOrEmpty(), UserMessage.FULLNAME_REQUIRED)
                .Filter(d => !d.user.Email.IsNullOrEmpty(), UserMessage.EMAIL_REQUIRED)
                .FilterAsync(async d => !(await userService.AnyPhoneNumberAsync(d.user.PhoneNumber)), string.Format(UserMessage.PHONE_EXISTED, user.PhoneNumber))
                .FilterAsync(async d => !(await userService.AnyEmailAsync(d.user.Email)), string.Format(UserMessage.EMAIL_EXISTED, user.Email))
                .FlatMapAsync(async d =>
                {
                    var result = await userService.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        result = await userService.AddToRoleAsync(user, RoleName.ACTIVE_USER);
                        if (result.Succeeded)
                        {
                            var claimsPrincipal = await userService.CreateClaimIdentityAsync(user);
                            return Option.Some<ClaimsPrincipal, string>(new ClaimsPrincipal(claimsPrincipal));
                        }
                        else
                        {
                            return Option.None<ClaimsPrincipal, string>(UserMessage.REGISTER_AUTH_FAILED);
                        }
                    }
                    else
                    {

                        return Option.None<ClaimsPrincipal, string>(UserMessage.PASSWORD_REGEX_FAIL);
                    }
                });
        }

        public async Task<Option<string, string>> GetResettingPasswordAsync(string userIdentification, string scheme, string host)
        {
            return await userIdentification
                .SomeNotNull()
                .WithException(UserMessage.EMAIL_REQUIRED)
                .Filter(uI => !uI.IsNullOrEmpty(), UserMessage.EMAIL_REQUIRED)
                .Map(uI => uI.Trim().ToLower())
                .FilterAsync(async uI => await this.userService.Users.AsNoTracking().AnyAsync(x => x.Email.Trim() == uI || x.PhoneNumber.Trim() == uI), string.Format(UserMessage.PHONE_EMAIL_NOT_EXISTED, userIdentification))
                .MapAsync(async uI =>
                {
                    var user = await this.userService.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email.Trim() == uI || x.PhoneNumber.Trim() == uI);
                    var resetToken = await this.userService.GeneratePasswordResetTokenAsync(user);
                    // Send reset password token to user mail
                    var currentBrand = await brandService.FirstOrDefaultAsync();
                    var subject = string.Format(SendEmailProperty.RESET_PASSWORD_SUBJECT, currentBrand.Name);
                    var mailPayloadInfor = new MailPayloadInformation
                    {
                        Mail = user.Email,
                        Name = user.Fullname
                    };

                    var body = string.Format(SendEmailProperty.RESET_PASSWORD_BODY, scheme, host, uI, resetToken);

                    emailService.SendEmailWithBachMocWrapper(subject, body, mailPayloadInfor);
                    return SendEmailProperty.RESET_PASSWORD_REQUEST_SUCCESS;
                });
        }

        public async Task<Option<string, string>> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel)
        {
            return await resetPasswordViewModel
                .SomeNotNull()
                .WithException(UserMessage.RESET_PASSWORD_NULL_REQUEST)
                .Filter(d => !d.UserIdentification.IsNullOrEmpty(), UserMessage.RESET_PASSWORD_IDENTIFICATION_NULL)
                .Filter(d => !d.ResetPasswordToken.IsNullOrEmpty(), UserMessage.RESET_PASSWORD_TOKEN_NULL)
                .Map(d =>
                {
                    d.UserIdentification = d.UserIdentification.Trim();
                    return d;
                })
                .FilterAsync(async d => await this.userService.Users.AsNoTracking().AnyAsync(x => x.Email.Trim() == d.UserIdentification || x.PhoneNumber.Trim() == d.UserIdentification), string.Format(UserMessage.PHONE_EMAIL_NOT_EXISTED, resetPasswordViewModel.UserIdentification))
                .FlatMapAsync(async d =>
                {
                    d.ResetPasswordToken = d.ResetPasswordToken.Replace(" ", "+");
                    var user = await this.userService.Users.FirstOrDefaultAsync(x => x.Email.Trim() == d.UserIdentification || x.PhoneNumber.Trim() == d.UserIdentification);

                    var result = await this.userService.ResetPasswordAsync(user, d.ResetPasswordToken, d.NewPassword);

                    if (result.Succeeded)
                    {
                        return Option.Some<string, string>(UserMessage.RESET_PASSWORD_SUCCESS);
                    }
                    return Option.None<string, string>(UserMessage.RESET_PASSWORD_FAIL);
                });
        }

        public async Task<Option<string, string>> ChangePasswordAsync(string userId, ChangePasswordViewMode request)
        {
            return await request
                .SomeNotNull()
                .WithException(UserMessage.RESET_PASSWORD_NULL_REQUEST)
                .Filter(d => !d.CurrentPassword.IsNullOrEmpty(), UserMessage.PASSWORD_REQUIRED)
                .Filter(d => !d.NewPassword.IsNullOrEmpty(), UserMessage.NEW_PASSWORD_REQUIRED)
                .FlatMapAsync(async d =>
                {
                    var user = await this.userService.FindByIdAsync(userId);
                    var result = await this.userService.ChangePasswordAsync(user, d.CurrentPassword, d.NewPassword);
                    if (result.Succeeded)
                    {
                        return Option.Some<string, string>(UserMessage.RESET_PASSWORD_SUCCESS);
                    }
                    return Option.None<string, string>(UserMessage.RESET_PASSWORD_FAIL);
                });
        }

        public async Task<Option<string, string>> ChangeDetailsAsync(string userId, UpdateDetailViewModel request)
        {
            return await request
                .SomeNotNull()
                .WithException(UserMessage.CHANGE_DETAIL_NULL_REQUEST)
                .Filter(d => !d.Fullname.IsNullOrEmpty(), UserMessage.FULLNAME_REQUIRED)
                .Filter(d => !d.PhoneNumber.IsNullOrEmpty(), UserMessage.PHONE_REQUIRED)
                .FlatMapAsync(async d =>
                {
                    var user = await this.userService.FindByIdAsync(userId);
                    if (
                        user.PhoneNumber != d.PhoneNumber
                        && await userService.AnyPhoneNumberAsync(d.PhoneNumber)
                    )
                    {
                        return Option.None<string, string>(string.Format(UserMessage.PHONE_EXISTED, d.PhoneNumber));
                    }
                    user.PhoneNumber = d.PhoneNumber;
                    user.Fullname = d.Fullname;

                    var result = await this.userService.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Option.Some<string, string>(UserMessage.CHANGE_DETAIL_SUCCESS);
                    }
                    return Option.None<string, string>(UserMessage.CHANGE_DETAIL_FAIL);
                });
        }

        public async Task<Option<EcommerceIdentityUser, string>> CreateNewUserAsync(AdminEditNewUserViewModel request)
        {
            return await request
                .SomeNotNull()
                .WithException(UserMessage.REGISTER_NULL_REQUEST)
                .Filter(data => !data.PhoneNumber.IsNullOrEmpty(), UserMessage.PHONE_REQUIRED)
                .Filter(data => !data.Fullname.IsNullOrEmpty(), UserMessage.FULLNAME_REQUIRED)
                .Filter(data => !data.Email.IsNullOrEmpty(), UserMessage.EMAIL_REQUIRED)
                .Filter(data => !data.Password.IsNullOrEmpty(), UserMessage.PASSWORD_REQUIRED)
                .Filter(data => data.ConfirmPassword == data.Password, UserMessage.CONFIRM_PASSWORD_NOT_MATCH)
                .FlatMapAsync(async data =>
                {
                    var user = new EcommerceIdentityUser();
                    user.PhoneNumber = request.PhoneNumber;
                    user.Email = request.Email;
                    user.Fullname = request.Fullname;
                    user.UserName = request.UserName;
                    user.NormalizedUserName = request.UserName;
                    user.Address = request.Address;
                    user.Gender = request.Gender;
                    var createResult = await this.userService.CreateAsync(user, request.Password);
                    if (createResult.Succeeded)
                    {
                        createResult = await this.userService.AddToRolesAsync(user, data.RoleIds);
                        return Option.Some<EcommerceIdentityUser, string>(user);
                    }
                    else
                    {
                        return Option.None<EcommerceIdentityUser, string>(string.Format(UserMessage.CREATE_NEW_FAILED, user.UserName));
                    }
                });
        }

        public async Task<Option<EcommerceIdentityUser, string>> UpdateUserAsync(AdminEditNewUserViewModel request)
        {
            return await request
               .SomeNotNull()
               .WithException(UserMessage.REGISTER_NULL_REQUEST)
               .Filter(data => !data.PhoneNumber.IsNullOrEmpty(), UserMessage.PHONE_REQUIRED)
               .Filter(data => !data.Fullname.IsNullOrEmpty(), UserMessage.FULLNAME_REQUIRED)
               .Filter(data => !data.Email.IsNullOrEmpty(), UserMessage.EMAIL_REQUIRED)
               .FilterAsync(async data => (await userService.FindByNameAsync(data.UserName)) != null, UserMessage.USER_NOT_EXISTED)
               .FlatMapAsync(async data =>
               {
                   var allRoles = await this.roleService.Roles.AsNoTracking().ToListAsync();

                   var currentUser = await userService.FindByNameAsync(data.UserName);
                   currentUser.PhoneNumber = request.PhoneNumber;
                   currentUser.Email = request.Email;
                   currentUser.Fullname = request.Fullname;
                   currentUser.UserName = request.UserName;
                   currentUser.NormalizedUserName = request.UserName;
                   currentUser.Address = request.Address;
                   currentUser.Gender = request.Gender;

                   var updateResult = await userService.UpdateAsync(currentUser);
                   if (updateResult.Succeeded)
                   {
                       var listCurrentUserRoles = await userService.GetRolesAsync(currentUser);
                       listCurrentUserRoles = listCurrentUserRoles.Select(x => allRoles.FirstOrDefault(y => y.Name == x).NormalizedName).ToList();
                       await userService.RemoveFromRolesAsync(currentUser, listCurrentUserRoles);
                       await userService.AddToRolesAsync(currentUser, data.RoleIds);
                       return Option.Some<EcommerceIdentityUser, string>(currentUser);
                   }
                   else
                   {
                       return Option.None<EcommerceIdentityUser, string>(string.Format(UserMessage.UPDATE_USER_FAIL, data.PhoneNumber));
                   }
               });
        }

        public async Task<ImmutableList<EcommerceIdentityRoleViewModel>> GetAllRolesAsync()
        {
            return (await this.roleService.Roles.ProjectTo<EcommerceIdentityRoleViewModel>(mapper.ConfigurationProvider).ToListAsync()).ToImmutableList();
        }
    }
}