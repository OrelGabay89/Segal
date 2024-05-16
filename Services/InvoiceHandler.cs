
using IsraelTax.Shared.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Configuration;
using CsvHelper.Configuration;
using CsvHelper;

namespace Invoices
{
    public class InvoiceService
    {
        public async Task RequestInvoiceNum(IConfiguration configuration)
        {
            string directoryPath = configuration["AppSettings:DirectoryPath"];
            Console.WriteLine("Reading csv file in path..." + directoryPath);
            string filePath = Path.Combine(directoryPath, configuration["AppSettings:FileName"]);

            // Ensure directory exists
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Directory does not exist, creating directory...");
                Directory.CreateDirectory(directoryPath);
                Console.WriteLine("Directory created at " + directoryPath);
            }

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: File does not exist at " + filePath);
                return;
            }

            // Check if the server is alive before proceeding
            var isAliveUrl = configuration["AppSettings:IsAliveUrl"];
            if (!await IsServerAlive(isAliveUrl))
            {
                Console.WriteLine("Server is not responding. Exiting process.");
                return;
            }

            Console.WriteLine("Server is alive. Start reading file...");
            var records = ReadDataFromCsv(filePath);
            var httpClient = new HttpClient();

            foreach (var record in records)
            {
                if (!ValidateData(record))
                {
                    Console.WriteLine("Validation error");
                    continue;
                }

                string url = configuration["AppSettings:ProcessingUrl"];
                Console.WriteLine($"URL from config: {url}");
                Console.WriteLine("Start requesting server...");

                var response = await SendDataToServer(httpClient, url, record);
                Console.WriteLine($"Server response for {record.Id}: {response}");
            }
        }

        private static async Task<bool> IsServerAlive(string url)
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
                    Console.WriteLine($"Failed to reach server: {ex.Message}");
                    return false;
                }
            }
        }

        private static IEnumerable<InvoiceCSVData> ReadDataFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<InvoiceCSVData>();
                return new List<InvoiceCSVData>(records);
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
            return data != null && !string.IsNullOrEmpty(data.CustomerName) && !string.IsNullOrEmpty(data.Amount)
                                && !string.IsNullOrEmpty(data.InvoiceNumber);
        }
    }
}
