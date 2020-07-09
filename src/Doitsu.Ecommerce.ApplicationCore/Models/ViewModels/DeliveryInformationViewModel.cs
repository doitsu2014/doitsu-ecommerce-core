using AutoMapper;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.ApplicationCore.Models.ViewModels
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
