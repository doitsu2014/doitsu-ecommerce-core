using System.ComponentModel.DataAnnotations;
using Doitsu.Service.Core.Interfaces.EfCore;
using Microsoft.AspNetCore.Identity;

namespace Doitsu.Ecommerce.Core.Data.Identities
{
    public class EcommerceIdentityUser : IdentityUser<int>, IActivable
    {
        [MaxLength(255)]
        public string Fullname { get; set; }
        [MaxLength(350)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string ZipCode { get; set; }
        [MaxLength(100)]
        public string Country { get; set; }
        public int Gender { get; set; }
        public bool Active { get; set; }
    }
}
