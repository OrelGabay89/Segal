using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using IsraelTax.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services;
using Shared.Models;
using Utils;

namespace Invoices
{
    public class InvoiceService
    {
        private readonly ILogger<InvoiceService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        private string _accessToken;
        private string _refreshToken;
        private int _tokenExpiration;
        private readonly string _shaamItaUrl;

        public InvoiceService(ILogger<InvoiceService> logger, HttpClient httpClient, IConfiguration configuration, TokenService tokenService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
            _tokenService = tokenService;
            _shaamItaUrl = configuration["AppSettings:ShaamItaUrl"];
        }

        public async Task RequestInvoiceNum(IConfiguration configuration)
        {
            string directoryPath = configuration["AppSettings:DirectoryPath"];
            Console.WriteLine("Reading csv file in path..." + directoryPath);
            string filePath = Path.Combine(directoryPath, configuration["AppSettings:FileName"]);

            if (!Directory.Exists(directoryPath))
            {
                _logger.LogInformation("Directory does not exist, creating directory...");
                Directory.CreateDirectory(directoryPath);
                _logger.LogInformation("Directory created at " + directoryPath);
            }

            if (!File.Exists(filePath))
            {
                _logger.LogError("Error: File does not exist at " + filePath);
                return;
            }

            var records = ReadDataFromCsv(filePath);

            var token = (await _tokenService.GetTokensAsync()).FirstOrDefault();
            if (token == null)
            {
                _logger.LogError("No token found in database.");
                return;
            }

            _accessToken = token.AccessToken;
            _refreshToken = token.RefreshToken;
            _tokenExpiration = token.ExpiresIn;

            var validRecords = new List<InvoiceCSVData>();
            var invalidRecords = new List<(InvoiceCSVData, List<string>)>();

            foreach (var record in records)
            {
                if (InvoiceCSVDataValidator.Validate(record, out var validationErrors))
                {
                    validRecords.Add(record);
                }
                else
                {
                    invalidRecords.Add((record, validationErrors));
                }
            }

            if (invalidRecords.Count > 0)
            {
                // Handle invalid records as needed, for example logging the errors
                foreach (var (record, errors) in invalidRecords)
                {
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Record with Invoice ID {record.Invoice_ID} has validation error: {error}");
                    }
                }
                return; // Exit the function if there are invalid records
            }

            foreach (var record in validRecords)
            {
                _logger.LogInformation("Start requesting server...");

                try
                {
                    var invoiceNumber = await RequestInvoiceNum(record);
                    _logger.LogInformation($"Server response for {record.Invoice_ID}: {invoiceNumber}");
                    record.ConfirmationNumber = invoiceNumber; // Assuming you have a field for invoice number
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error requesting invoice number: {ex.Message}");
                }
            }

            WriteDataToCsv(filePath, records);
        }

        private void WriteDataToCsv(string filePath, IEnumerable<InvoiceCSVData> records)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (field) => true,
                Delimiter = ","
            };

            using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(true))) // Add BOM for UTF-8
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(records);
            }
        }



        private IEnumerable<InvoiceCSVData> ReadDataFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                BadDataFound = context =>
                {
                    Console.WriteLine($"Bad data found on row {context.RawRecord}");
                },
                HeaderValidated = null,
                ReadingExceptionOccurred = context =>
                {
                    Console.WriteLine($"Reading exception: {context.Exception.Message}");
                    return true;
                }
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<InvoiceCSVData>().ToList();
                Console.WriteLine($"Records read: {records.Count}");
                return records;
            }
        }

        public async Task<string> RequestInvoiceNum(InvoiceCSVData invoiceDetails)
        {
            try
            {
                var invoiceNumber = await GetInvoiceNumberWithAccessToken(invoiceDetails);
                _logger.LogInformation($"Invoice number received: {invoiceNumber}");
                return invoiceNumber;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Access token expired. Refreshing token...");

                try
                {

                    await RefreshAccessToken();
                    var invoiceNumber = await GetInvoiceNumberWithAccessToken(invoiceDetails);
                    _logger.LogInformation($"Invoice number received: {invoiceNumber}");
                    return invoiceNumber;
                }
                catch (Exception innerEx)
                {
                    _logger.LogError($"Error requesting invoice number after refreshing token: {innerEx.Message}");
                    return "000000000";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error requesting invoice number: {ex.Message}");
                throw;
            }
        }

        public async Task RefreshAccessToken()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _configuration["AppSettings:RefreshTokenUrl"]);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _configuration["AppSettings:ClientId"]),
                new KeyValuePair<string, string>("client_secret", _configuration["AppSettings:ClientSecret"]),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", _refreshToken)
    });

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JObject.Parse(responseContent);
            _accessToken = tokenResponse["access_token"].ToString();
            _tokenExpiration = tokenResponse["expires_in"].ToObject<int>();

            _refreshToken = tokenResponse["refresh_token"]?.ToString() ?? _refreshToken;
            var refreshTokenExpiration = tokenResponse["refresh_token_expires_in"]?.ToObject<int>() ?? 7546005;

            // Mark existing tokens as revoked
            var filter = Builders<Token>.Filter.Empty;
            var update = Builders<Token>.Update.Set(t => t.Status, "Revoked");
            await _tokenService.UpdateTokenAsync(filter, update);

            // Insert the new token
            var newToken = new Token
            {
                TokenType = "Bearer",
                AccessToken = _accessToken,
                ExpiresIn = _tokenExpiration,
                RefreshToken = _refreshToken,
                RefreshTokenExpiresIn = refreshTokenExpiration,
                Status = "Active"
            };

            await _tokenService.InsertTokenAsync(newToken);
        }

        private async Task<string> GetInvoiceNumberWithAccessToken(InvoiceCSVData invoice)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://ita-api.taxes.gov.il/shaam/tsandbox/Invoices/v1/Approval");
                request.Headers.Add("Authorization", "Bearer " + _accessToken);

                var jsonPayload = $@"
                {{
                   ""Invoice_ID"": ""{invoice.Invoice_ID}"",
                   ""Invoice_Type"": {invoice.Invoice_Type},
                   ""Vat_Number"": {invoice.Vat_Number},
                   ""Union_Vat_Number"": {invoice.Union_Vat_Number},
                   ""Invoice_Reference_Number"": ""{invoice.Invoice_Reference_Number}"",
                   ""Customer_VAT_Number"": {invoice.Customer_VAT_Number},
                   ""Customer_Name"": ""{invoice.Customer_Name}"",
                   ""Invoice_Date"": ""{invoice.Invoice_Date}"",
                   ""Invoice_Issuance_Date"": ""{invoice.Invoice_Issuance_Date}"",
                   ""Branch_ID"": ""{invoice.Branch_ID}"",
                   ""Accounting_Software_Number"": {invoice.Accounting_Software_Number},
                   ""Client_Software_Key"": ""{invoice.Client_Software_Key}"",
                   ""Amount_Before_Discount"": {invoice.Amount_Before_Discount},
                   ""Discount"": {invoice.Discount},
                   ""Payment_Amount"": {invoice.Payment_Amount},
                   ""VAT_Amount"": {invoice.VAT_Amount},
                   ""Payment_Amount_Including_VAT"": {invoice.Payment_Amount_Including_VAT},
                   ""Invoice_Note"": ""{invoice.Invoice_Note}"",
                   ""Action"": {invoice.Action},
                   ""Vehicle_License_Number"": {invoice.Vehicle_License_Number},
                   ""Phone_Of_Driver"": ""{invoice.Phone_Of_Driver}"",
                   ""Arrival_Date"": ""{invoice.Arrival_Date}"",
                   ""Estimated_Arrival_Time"": ""{invoice.Estimated_Arrival_Time}"",
                   ""Transition_Location"": {invoice.Transition_Location},
                   ""Delivery_Address"": ""{invoice.Delivery_Address}"",
                   ""Additional_Information"": {invoice.Additional_Information}
                }}";


                var content = new StringContent(jsonPayload, System.Text.Encoding.Default, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                //var request = new HttpRequestMessage(HttpMethod.Post, "https://ita-api.taxes.gov.il/shaam/tsandbox/Invoices/v1/Approval");
                //request.Content = new StringContent(invoiceDetails, System.Text.Encoding.UTF8, "application/json");

                //HttpResponseMessage response = await _httpClient.SendAsync(request);
                //response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                JObject invoiceResponse = JObject.Parse(responseContent);
                return invoiceResponse["Confirmation_Number"].ToString();
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError($"HTTP request error: {httpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting invoice number: {ex.Message}");
                throw;
            }
        }
    }
}
