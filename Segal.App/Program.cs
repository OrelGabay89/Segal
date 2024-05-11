
using IsraelTax.App;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services;
using System.Reflection;


namespace CSVReaderToServer
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
            // Begin your application code
            logger.LogInformation("Application Starting...");

            // Get the directory of the currently executing application
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string licenseFilePath = Path.Combine(exePath, "license.lic");

            var config = serviceProvider.GetRequiredService<IConfiguration>();
            string customerIdentifier = config["LicenseSettings:CustomerIdentifier"];

            if (!LicenseManager.VerifyLicense(licenseFilePath, customerIdentifier))
            {
                Console.ReadLine();
                return;
            }

#if DEBUG
            //args = new string[] { "--sendmail", "Test Subject", "body", "orelgabay89@gmail.com", "C:\\Users\\OrelGabay\\Desktop\\test.txt" };
            // Parse command line arguments for mail sending
            if (args.Length > 0 && args[0].ToLower() == "--sendmail" && args.Length <= 5)
            {
                SendMail(args, serviceProvider, logger);
                Console.ReadLine();
                return;
            }

#endif
            // Check if the first argument is "--sendsms" and there are at least four arguments
            if (args.Length >= 4 && args[0].ToLower() == "--sendsms")
            {
                SendSms(args, serviceProvider, logger);
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
                        var invoiceHandler = serviceProvider.GetRequiredService<InvoiceHandler>();
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

        private static void SendMail(string[] args, ServiceProvider serviceProvider, ILogger<Program> logger)
        {
            // Help for sending Mail
            if (args.Length >= 2 && args[0].ToLower() == "--help" && args[1].ToLower() == "mail")
            {
                Console.WriteLine("Help for Mail Sending Command:");
                Console.WriteLine("--sendmail [subject] [email address] [body]");
                Console.WriteLine("  subject: The subject line of the email.");
                Console.WriteLine("  email address: The recipient's email address.");
                Console.WriteLine("  body: The main content of the email.");
                Console.WriteLine("Example:");
                Console.WriteLine("  --sendmail \"Meeting Reminder\" \"example@example.com\" \"Reminder: Meeting at 3 PM Today\"");
                return;
            }


            var mailSender = serviceProvider.GetRequiredService<MailService>();
            string subject = args.Length > 1 ? args[1] : "Default Subject";
            string body = args.Length > 2 ? args[2] : "Default Body";
            string recipient = args.Length > 3 ? args[3] : "default@example.com";
            string attachmentPath = args.Length > 4 ? args[4] : null;

            mailSender.SendEmailUsingOutlook(subject, body, recipient, attachmentPath);

            return;

        }

        private async static void SendSms(string[] args, ServiceProvider serviceProvider, ILogger<Program> logger)
        {
            // Check if the first argument is "--help" and the second argument is "sms"
            if (args.Length >= 2 && args[0].ToLower() == "--help" && args[1].ToLower() == "sms")
            {
                Console.WriteLine("Help for SMS Sending Command:");
                Console.WriteLine("--sendsms [message] [phone number] [name]");
                Console.WriteLine("  message: Text of the SMS to send.");
                Console.WriteLine("  phone number: The recipient's phone number.");
                Console.WriteLine("  name: The recipient's name.");
                Console.WriteLine("Example:");
                Console.WriteLine("  --sendsms \"Hello, this is a test.\" \"0542559101\" \"Segal\"");
                return;
            }


            logger.LogInformation("Attempting to send sms...");

            // Extracting message, phone number, and name from the arguments
            string message = args[1];
            string phoneNumber = args[2];
            string name = args[3];

            try
            {
                // Get the SMS service from the service provider
                var smsSender = serviceProvider.GetRequiredService<SmsService>();
                // Send the SMS using the extracted parameters
                await smsSender.SendSmsAsync(message, phoneNumber, name);
                logger.LogInformation("SMS sent to {Name} at {PhoneNumber}. Message: {Message}", name, phoneNumber, message);
            }
            catch (Exception ex)
            {
                // Log the error if something goes wrong
                logger.LogError(ex, "Failed to send SMS");
            }

            return;

        }

        private static void ConfigureServices(IServiceCollection services)
        {
            IServiceCollection serviceCollection = services.AddLogging(configure => configure.AddConsole())
         .AddTransient<MailService>()
         .AddSingleton<SmsService>()
         .AddSingleton<InvoiceHandler>()
         .AddHttpClient()  // This registers IHttpClientFactory which allows you to use HttpClient
         .AddSingleton<IConfiguration>(new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .Build());
        }
    }
}