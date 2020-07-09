using Newtonsoft.Json;
using System;

namespace Doitsu.Ecommerce.ApplicationCore.AuthorizeBuilder
{
    /// <summary>
    /// The model present to the response when client request authorize to your web app
    /// </summary>
    public class TokenAuthorizeModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("validTo")]
        public DateTime ValidTo { get; set; }
        [JsonProperty("validFrom")]
        public DateTime ValidFrom { get; set; }
    }
}
