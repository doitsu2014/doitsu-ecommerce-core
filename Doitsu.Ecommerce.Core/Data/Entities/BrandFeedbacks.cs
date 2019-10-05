using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class BrandFeedbacks : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
        public string AvatarUrl { get; set; }
        public int? UserId { get; set; }

        public virtual EcommerceIdentityUser User { get; set; }
    }
}
