using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Models;

namespace Services
{
    public class SmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly IConfiguration _configuration; // Store the IConfiguration to use later

        public SmsService(ILogger<SmsService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration; // Assign configuration to a private field
            _apiUrl = configuration["SmsSettings:ApiUrl"];

            var credentialSection = configuration.GetSection("SmsSettings:ApiCredentials");

        }

        public async Task SendSmsAsync(string message, string phoneNumber, string senderName)
        {
            if (senderName.Length > 11)
            {
                throw new ArgumentException("Sender name must be 11 characters or less.", nameof(senderName));
            }

            var credentials = _configuration.GetSection("SmsSettings:ApiCredentials").Get<ApiCredential[]>();
            var credential = credentials.FirstOrDefault(c => c.name.Equals(senderName, StringComparison.OrdinalIgnoreCase));

            if (credential == null)
            {
                _logger.LogError($"No credentials found for sender: {senderName}");
                throw new InvalidOperationException($"No credentials found for sender: {senderName}");
            }

            _logger.LogInformation($"Using credentials for sender: {senderName}");

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
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credential.token);
                var response = await _httpClient.PostAsync(_apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"SMS sent successfully to {phoneNumber}. Response: {responseBody}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS.");
                throw;
            }
        }
    }
}
