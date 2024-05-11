using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Services
{
    public class SmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _apiCredentials;

        public SmsService(ILogger<SmsService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiUrl = configuration["SmsSettings:ApiUrl"];
            _apiCredentials = configuration["SmsSettings:ApiCredentials"];
        }

        public async Task SendSmsAsync(string message, string phoneNumber, string senderName)
        {
            var requestData = new
            {
                Data = new
                {
                    Message = message,
                    Recipients = new[] { new { Phone = phoneNumber } },
                    Settings = new { Sender = senderName }
                }
            };

            string json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _apiCredentials);
                var response = await _httpClient.PostAsync(_apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("SMS sent successfully. Response: {responseBody}", responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS.");
            }
        }
    }
}
