using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Service.Core.Interfaces.EfCore;
using Microsoft.AspNetCore.Identity;

namespace Doitsu.Ecommerce.Core.Abstraction.Identities
{
    public class EcommerceIdentityUser : IdentityUser<int>, IActivable
    {
        [MaxLength(125)]
        public string Fullname { get; set; }
        [MaxLength(255)]
        public string Address { get; set; }
        [MaxLength(15)]
        public string ZipCode { get; set; }
        [MaxLength(60)]
        public string Country { get; set; }
        [MaxLength(60)]
        public string City { get; set; }
        [MaxLength(30)]
        public string District { get; set; }
        [MaxLength(30)]
        public string Ward { get; set; }

        [DefaultValue(GenderEnum.Unknown)]
        public int Gender { get; set; }
        public bool Active { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        [DefaultValue(0)]
        public decimal Balance { get; set; }

        public virtual ICollection<Blogs> BlogsCreater { get; set; }
        public virtual ICollection<Blogs> BlogsPublisher { get; set; }
        public virtual ICollection<BrandFeedbacks> BrandFeedbacks { get; set; }
        public virtual ICollection<CustomerFeedbacks> CustomerFeedbacks { get; set; }
        public virtual ICollection<MarketingCustomers> MarketingCustomers { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<UserTransaction> UserTransactions { get; set; }
        public virtual ICollection<DeliveryInformation> DeliveryInformations { get; set; }

        public virtual ICollection<IdentityUserRole<int>> UserRoles { get; set; }
    }
}
