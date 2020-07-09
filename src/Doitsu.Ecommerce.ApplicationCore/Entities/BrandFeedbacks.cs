using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
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
