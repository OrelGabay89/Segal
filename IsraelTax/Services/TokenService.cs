using System.Text;
using IsraelTax.Interfaces;
using IsraelTax.Shared.Models;
using Newtonsoft.Json;

namespace IsraeliTaxTokenFetcher.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;

        public TokenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> FetchTokenUsingCode(string requestBody)
        {
            var code = ExtractCodeFromRequestBody(requestBody);
            if (string.IsNullOrEmpty(code))
            {
                return "No authorization code provided.";
            }

            var tokenUrl = "https://ita-api.taxes.gov.il/shaam/tsandbox/onetimetoken/oauth2/token";
            var clientId = "your_client_id";
            var clientSecret = "your_client_secret";
            var redirectUri = "http://localhost:5001/success";

            var content = new StringContent($"grant_type=authorization_code&code={code}&redirect_uri={Uri.EscapeDataString(redirectUri)}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            var response = await _httpClient.PostAsync(tokenUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonContent);
                return tokenResponse.AccessToken;
            }
            else
            {
                return $"Failed to fetch token. Status: {response.StatusCode}";
            }
        }

        private string ExtractCodeFromRequestBody(string requestBody)
        {
            // Implement this method to parse the requestBody to extract the code.
            // This is a placeholder implementation.
            // You might use JSON parsing or form-data parsing depending on the format.
            return requestBody;  // Update this line based on actual requestBody format.
        }
    }
}