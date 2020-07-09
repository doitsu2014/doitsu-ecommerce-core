using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;

namespace Doitsu.Ecommerce.ApplicationCore.Models.ViewModels
{
    public class EcommerceIdentityRoleViewModel : EcommerceIdentityRole
    {
        public override int Id { get; set; }
        public override string Name { get; set; }
        public override string NormalizedName { get; set; }
    }
}