using System.Text.Json.Serialization;

namespace MattSwain.Umbraco.Commerce.ShippingProviders.FedEx.Api.Models
{
    public class FedExOauthTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
