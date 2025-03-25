using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Umbraco.Commerce.Extensions;
using MattSwain.Umbraco.Commerce.ShippingProviders.FedEx.Api.Models;
using System.Net.Http.Json;
using System.Threading;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace MattSwain.Umbraco.Commerce.ShippingProviders.FedEx.Api
{
    public class FedExExpressClient
    {
        private readonly HttpClient _httpClient;
        private readonly string ratesUrl = "/rate/v1/comprehensiverates/quotes";
        private readonly string tokenUrl = "/oauth/token";
        private string _baseAddress = "";
        private readonly IMemoryCache _cache;
        private const string CACHEKEY = "FedExToken";

        public static FedExExpressClient Create(IHttpClientFactory httpClientFactory, FedExSettings settings, IMemoryCache cache)
            => new FedExExpressClient(httpClientFactory.CreateClient(), settings, cache);

        private FedExExpressClient(HttpClient httpClient, FedExSettings settings, IMemoryCache cache)
        {
            _cache = cache;

            settings.MustNotBeNull(nameof(settings));

            settings.ClientId.MustNotBeNullOrWhiteSpace(nameof(settings.ClientId));
            settings.ClientSecret.MustNotBeNullOrWhiteSpace(nameof(settings.ClientSecret));

            _baseAddress = settings.TestMode ? "https://apis-sandbox.fedex.com" : "https://apis.fedex.com";

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_baseAddress);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FedExOauthTokenResponse> GetToken(string clientId, string clientSecret, CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(CACHEKEY, out string cachedToken))
            {
                return new FedExOauthTokenResponse { AccessToken = cachedToken };
            }

            var requestBody = new StringContent(
                  $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}",
                  Encoding.UTF8,
                  "application/x-www-form-urlencoded"
              );

            var tokenUri = new Uri($"{_baseAddress}{tokenUrl}");

            using (var resp = await _httpClient.PostAsync(tokenUri, requestBody).ConfigureAwait(false))
            {
                resp.EnsureSuccessStatusCode();
                var tokenResponse = await resp.Content.ReadFromJsonAsync<FedExOauthTokenResponse>(cancellationToken).ConfigureAwait(false);

                if (tokenResponse?.AccessToken == null)
                {
                    throw new InvalidOperationException("Failed to retrieve a valid token from FedEx.");
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn)
                };
                _cache.Set(CACHEKEY, tokenResponse.AccessToken, cacheOptions);

                return tokenResponse;
            }
        }

        public async Task<FedExExpressShippingRatesResponse> GetRatesAsync(FedExShippingRatesRequest req, string token = "", CancellationToken cancellationToken = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string json = JsonSerializer.Serialize(req);

            using (var resp = await _httpClient.PostAsJsonAsync($"{_baseAddress}{ratesUrl}", req, cancellationToken).ConfigureAwait(false))
            {
                return await resp.Content.ReadFromJsonAsync<FedExExpressShippingRatesResponse>(cancellationToken).ConfigureAwait(false);
            }
        }

    }
}
