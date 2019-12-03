using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class UserTransactionViewModel
    {
        public string Description { get; set; }
        public UserTransactionTypeEnum Type { get; set; }
        public DateTime CreatedTime { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}