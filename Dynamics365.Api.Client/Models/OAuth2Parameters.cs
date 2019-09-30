using Newtonsoft.Json;

namespace Dynamics365.Api.Client.Models
{
    public class OAuth2Parameters
    {
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType { get; set; } = "password";

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; } = "https://graph.microsoft.com/.default";
        
        [JsonProperty(PropertyName = "userName")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}
