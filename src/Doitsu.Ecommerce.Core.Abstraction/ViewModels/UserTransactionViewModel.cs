using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.Abstraction.ViewModels
{
    public class UserTransactionViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("type")]
        public UserTransactionTypeEnum Type { get; set; }
        
        [JsonProperty("createdTime")]
        public DateTime CreatedTime { get; set; }
        
        [JsonProperty("currentBalance")]
        public decimal CurrentBalance { get; set; }
        
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        
        [JsonProperty("sign")]
        public UserTransactionSignEnum Sign { get; set; }
        
        [JsonProperty("destinationBalance")]
        public decimal DestinationBalance { get; set; }

        [JsonProperty("order")]
        public virtual OrderDetailViewModel Order { get; set; }
    }
}