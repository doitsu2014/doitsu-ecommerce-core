using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.ApplicationCore.Models.ViewModels
{
    public class EcommerceIdentityUserViewModel : EcommerceIdentityUser
    {
        [JsonProperty("id")]
        public override int Id { get; set; }

        [JsonProperty("userName")]
        public override string UserName { get; set; }

        [JsonProperty("email")]
        public override string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public override string PhoneNumber { get; set; }

        [JsonProperty("fullName")]
        public new  string Fullname { get; set; }

        [JsonProperty("address")]
        public new string Address
        {
            get;
            set;
        }

        [JsonProperty("balance")]
        public new decimal Balance
        {
            get;
            set;
        }

        [JsonProperty("gender")]
        public new int Gender
        {
            get;
            set;
        }
        
        [JsonProperty("deliveryInformation")]
        public new List<DeliveryInformationViewModel> DeliveryInformations { get; set; }

        [JsonProperty("roleName")]
        public List<EcommerceIdentityRoleViewModel> EcommerceIdentityRoles { get; set; } 
    }
}