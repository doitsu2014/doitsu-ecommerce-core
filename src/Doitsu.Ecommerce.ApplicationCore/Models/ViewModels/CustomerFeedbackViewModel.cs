using System;
using System.ComponentModel.DataAnnotations;

namespace Doitsu.Ecommerce.ApplicationCore.Models.ViewModels
{
    public class CustomerFeedbackViewModel
    {
        [Required]
        public string CustomerName
        {
            get;
            set;
        }

        [Required]
        public string Email
        {
            get;
            set;
        }

        [Required]
        public string Phone
        {
            get;
            set;
        }

        [Required]
        public string Content
        {
            get;
            set;
        }
        public CustomerFeedBackTypeEnum Type
        {
            get;
            set;
        }
    }

    public class CustomerFeedbackOverviewViewModel
    {
        public string CustomerName
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string Phone
        {
            get;
            set;
        }
        public DateTime CreatedDate
        {
            get;
            set;
        }
        public string Content
        {
            get;
            set;
        }
    }
}