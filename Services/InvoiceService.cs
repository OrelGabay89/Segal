
using IsraelTax.Shared.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Configuration;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Services;
using Shared.Services;

namespace Invoices
{
    public class InvoiceService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly string _apiUrl;
        private readonly IConfiguration _configuration;
        private readonly string _directoryPath;
        private readonly string _isAliveUrl;
        private readonly string _processingUrl;


        public InvoiceService(ILogger<SmsService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration; // Assign configuration to a private field
            _directoryPath =  configuration["AppSettings:DirectoryPath"];
            _apiUrl = configuration["SmsSettings:ApiUrl"];
            _isAliveUrl = configuration["AppSettings:IsAliveUrl"];
            _processingUrl = configuration["AppSettings:ProcessingUrl"];

        }

        public async Task RequestInvoiceNum(IConfiguration configuration)
        {
            string directoryPath = configuration["AppSettings:DirectoryPath"];
            Console.WriteLine("Reading csv file in path..." + directoryPath);
            string filePath = Path.Combine(directoryPath, configuration["AppSettings:FileName"]);

            // Ensure directory exists
            if (!Directory.Exists(directoryPath))
            {
                _logger.LogInformation("Directory does not exist, creating directory...");

                Directory.CreateDirectory(directoryPath);

                _logger.LogInformation("Directory created at " + directoryPath);

            }

            // Check if the file exists
            if (!File.Exists(filePath))
            {

                _logger.LogError("Error: File does not exist at " + filePath);
                return;
            }

            if (!await IsServerAlive(_isAliveUrl))
            {
                _logger.LogError("Server is not responding. Exiting process.");
                return;
            }

            _logger.LogInformation("Server is alive.Start reading file...");

            var records = ReadDataFromCsv(filePath);
            var httpClient = new HttpClient();

            foreach (var record in records)
            {
                if (!ValidateData(record))
                {
                    _logger.LogError("Validation error");

                    continue;
                }

                _logger.LogInformation($"URL from config: {_processingUrl}");
                _logger.LogInformation("Start requesting server...");

                var response = await SendDataToServer(httpClient, _processingUrl, record);

                _logger.LogInformation($"Server response for {record.invoice_id}: {response}");
            }
        }

        private async Task<bool> IsServerAlive(string url)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    return response.IsSuccessStatusCode;  // Check if HTTP status code is successful
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to reach server: {ex.Message}");

                    return false;
                }
            }
        }

        private IEnumerable<InvoiceCSVData> ReadDataFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                //NewLine = Environment.NewLine,
                MissingFieldFound = null, // Ignore missing fields
                BadDataFound = context =>
                {
                    // Log bad data
                    Console.WriteLine($"Bad data found on row {context.RawRecord}");
                },
                HeaderValidated = null, // Optionally ignore header validation issues
                ReadingExceptionOccurred = context =>
                {
                    // Log exceptions that occur during reading
                    Console.WriteLine($"Reading exception: {context.Exception.Message}");
                    return true; // Set to false if you want to stop processing on an exception
                }
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<InvoiceCSVDataMap>(); // Ensure you have a class map if needed
                var records = csv.GetRecords<InvoiceCSVData>().ToList();
                Console.WriteLine($"Records read: {records.Count}");
                return records;
            }

        }

        private static async Task<string> SendDataToServer(HttpClient client, string url, InvoiceCSVData data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }

        private static bool ValidateData(InvoiceCSVData data)
        {
            return true;
            //return data != null && !string.IsNullOrEmpty(data.Customer_Name) && data.Payment_Amount > 0
            //                    && !string.IsNullOrEmpty(data.Invoice_Reference_Number);
        }
    }
}
