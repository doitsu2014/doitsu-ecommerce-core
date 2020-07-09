using System;
using System.ComponentModel.DataAnnotations;

namespace Doitsu.Ecommerce.ApplicationCore.Models.ViewModels {
    public class MarketingCustomerViewModel {
        [Required]
        public string Email { get; set; }
    }

    public class MarketingCustomerMoreInformationViewModel {
        public string Email { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserFullname { get; set; }
        public DateTime JoinedDate { get; set; }
    }
}