using AutoMapper;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core.Abstraction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class DeliveryInformationViewModel : BaseViewModel<DeliveryInformation>
    {
        public DeliveryInformationViewModel()
        {
        }

        public DeliveryInformationViewModel(DeliveryInformation entity, IMapper mapper) : base(entity, mapper)
        {
        }
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("district")]
        public string District { get; set; }
        [JsonProperty("ward")]
        public string Ward { get; set; }
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("vers")]
        public byte[] Vers { get; set; }
    }
}
