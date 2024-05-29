using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SendSms
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Parse comma-separated arguments
            args = ParseCommaSeparatedArgs(args);

            // Build and run the host
            var host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;

            // Get logger
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("SendSms Application Starting...");

            await SendSms(args, serviceProvider, logger);
        }

        private static string[] ParseCommaSeparatedArgs(string[] args)
        {
            if (args.Length == 1 && args[0].Contains(','))
            {
                return args[0].Split(',');
            }
            return args;
        }

        private static async Task SendSms(string[] args, IServiceProvider serviceProvider, ILogger<Program> logger)
        {
            if (args.Length >= 1 && args[0].ToLower() == "--help")
            {
                Console.WriteLine("Help for SMS Sending Command:");
                Console.WriteLine("[phone number],[message],[name]");
                Console.WriteLine("phone number: The recipient's phone number.");
                Console.WriteLine("message: Text of the SMS to send.");
                Console.WriteLine("name: The recipient's name.");
                Console.WriteLine("Example:");
                Console.WriteLine("\"0542559101\",\"Hello, this is a test.\",\"Segal\"");
                return;
            }

            logger.LogInformation("Attempting to send SMS...");

            string phoneNumber = args.Length > 0 ? args[0] : "default number";
            string message = args.Length > 1 ? args[1] : "default message";
            string name = args.Length > 2 ? args[2] : "default name";

            try
            {
                var smsSender = serviceProvider.GetRequiredService<SmsService>();
                await smsSender.SendSmsAsync(message, phoneNumber, name);
                logger.LogInformation("SMS sent to {Name} at {PhoneNumber}. Message: {Message}", name, phoneNumber, message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send SMS");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services, context.Configuration, context.HostingEnvironment);
                });

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddLogging()
                    .AddHttpClient() // Register HttpClient
                    .AddTransient<SmsService>()
                    .AddSingleton<IConfiguration>(configuration);
        }
    }
}
