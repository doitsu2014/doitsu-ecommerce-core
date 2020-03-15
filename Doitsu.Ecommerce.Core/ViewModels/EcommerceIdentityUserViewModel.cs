using System.Collections.Generic;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class EcommerceIdentityUserViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("fullName")]
        public string Fullname { get; set; }
        [JsonProperty("address")]
        public string Address
        {
            get;
            set;
        }

        [JsonProperty("balance")]
        public decimal Balance
        {
            get;
            set;
        }

        [JsonProperty("gender")]
        public int Gender
        {
            get;
            set;
        }

        [JsonProperty("roleName")]
        public List<EcommerceIdentityRoleViewModel> EcommerceIdentityRoles { get; set; }
    }
}