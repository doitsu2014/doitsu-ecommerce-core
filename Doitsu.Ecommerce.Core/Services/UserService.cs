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

namespace Doitsu.Ecommerce.Core.Services
{

    public interface IUserService
    {
        Task<ImmutableList<EcommerceIdentityUserViewModel>> GetUsers(string name = "", string phone = "");
    }

    public class UserService : IUserService
    {
        private readonly EcommerceRoleIntManager<EcommerceIdentityRole> roleService;
        private readonly EcommerceIdentityUserManager<EcommerceIdentityUser> userService;
        private readonly IMapper mapper;
        public UserService(EcommerceRoleIntManager<EcommerceIdentityRole> roleService,
                           EcommerceIdentityUserManager<EcommerceIdentityUser> userService,
                           IMapper mapper)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.mapper = mapper;
        }
        public async Task<ImmutableList<EcommerceIdentityUserViewModel>> GetUsers(string userName = "", string phoneNumber = "")
        {
            var roleQuery = (await this.roleService.Roles.AsNoTracking().ToListAsync()).ToImmutableList();
            var userQuery = this.userService.Users.Include(u => u.UserRoles).AsNoTracking();
            var listFilterOfUser = new List<(bool filterable, Func<EcommerceIdentityUser, bool> function)>();
            listFilterOfUser.Add((!string.IsNullOrEmpty(userName), u => u.UserName == userName));
            listFilterOfUser.Add((!string.IsNullOrEmpty(phoneNumber), u => u.PhoneNumber == phoneNumber));
            listFilterOfUser.Where(exp => exp.filterable).ToList()
                .ForEach(exp => userQuery = userQuery.Where(exp.function).AsQueryable());

            return (await userQuery.ToListAsync())
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
    }

}