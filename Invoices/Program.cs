using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utils;


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

            // Get the directory of the currently executing application
            string exePath = AppContext.BaseDirectory;
            string licenseFilePath = Path.Combine(exePath, "license.lic");

            var config = serviceProvider.GetRequiredService<IConfiguration>();
            string customerIdentifier = config["LicenseSettings:CustomerIdentifier"];

            if (!LicenseManager.VerifyLicense(licenseFilePath, customerIdentifier))
            {
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Menu:");
            Console.WriteLine("1. Request an invoice number");
            Console.WriteLine("2. Exit");
            Console.Write("Enter your choice: ");

            while (true)
            {
                string userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    case "1":
                        logger.LogInformation("Requesting an invoice number...");
                        var invoiceHandler = serviceProvider.GetRequiredService<InvoiceService>();
                        await invoiceHandler.RequestInvoiceNum(config);
                        logger.LogInformation("End requesting an invoice number.");
                        break;
                    case "2":
                        logger.LogInformation("Exiting application...");
                        return;
                    default:
                        logger.LogWarning("Invalid choice, please try again.");
                        break;
                }
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                    .AddSingleton<InvoiceService>()
                    .AddHttpClient()
                    .AddSingleton<IConfiguration>(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build());
        }
    }
}
