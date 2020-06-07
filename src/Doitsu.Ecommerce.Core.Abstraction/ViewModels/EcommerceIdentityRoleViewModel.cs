using Doitsu.Ecommerce.Core.Abstraction.Identities;

namespace Doitsu.Ecommerce.Core.Abstraction.ViewModels
{
    public class EcommerceIdentityRoleViewModel : EcommerceIdentityRole
    {
        public override int Id { get; set; }
        public override string Name { get; set; }
        public override string NormalizedName { get; set; }
    }
}