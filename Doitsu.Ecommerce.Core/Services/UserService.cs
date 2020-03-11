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

namespace Doitsu.Ecommerce.Core.Services
{

    public interface IUserService
    {
        Task<ImmutableList<EcommerceIdentityUser>> GetUsers(string name = "", string phone = "");
    }

    public class UserService : IUserService
    {
        private readonly EcommerceRoleIntManager<EcommerceIdentityRole> roleService;
        private readonly EcommerceIdentityUserManager<EcommerceIdentityUser> userService;
        public UserService(EcommerceRoleIntManager<EcommerceIdentityRole> roleService,
                           EcommerceIdentityUserManager<EcommerceIdentityUser> userService)
        {
            this.roleService = roleService;
            this.userService = userService;
        }
        public Task<ImmutableList<EcommerceIdentityUser>> GetUsers(string userName = "", string phoneNumber = "")
        {
            var roleQuery = this.roleService.Roles.AsNoTracking();
            var userQuery = this.userService.Users.AsNoTracking();
            var listFilterOfUser = new List<(bool filterable, Func<EcommerceIdentityUser, bool> function)>();
            listFilterOfUser.Add((!string.IsNullOrEmpty(userName), u => u.UserName == userName));
            listFilterOfUser.Add((!string.IsNullOrEmpty(phoneNumber), u => u.PhoneNumber == phoneNumber));
            listFilterOfUser.Where(exp => exp.filterable).ToList()
                .ForEach(exp => userQuery = userQuery.Where(exp.function).AsQueryable());

            var joinTable = userQuery.Join(roleQuery, x => x.Id, y => y.);
            throw new System.NotImplementedException();
        }
    }

}