using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Invoices
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configure services and logging
            var services = new ServiceCollection();
            ConfigureServices(services);

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Get logger
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Application Starting...");

            var config = serviceProvider.GetRequiredService<IConfiguration>();
            string customerIdentifier = config["LicenseSettings:CustomerIdentifier"];

            logger.LogInformation("Requesting an invoice number...");
            var invoiceHandler = serviceProvider.GetRequiredService<InvoiceService>();
            await invoiceHandler.RequestInvoiceNum(config);
            logger.LogInformation("End requesting an invoice number.");
           
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                    .AddSingleton<InvoiceService>()
                    .AddSingleton<TokenService>()
                    .AddHttpClient()
                    .AddSingleton<IConfiguration>(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build());
        }
    }
}
