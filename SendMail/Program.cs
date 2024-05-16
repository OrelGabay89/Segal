using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;


namespace SendMail
{
    class Program
    {
        static async Task Main(string[] args)
        {
            args = ParseCommaSeparatedArgs(args);

            var host = CreateHostBuilder(args).Build();

            // Build the service provider
            var serviceProvider = host.Services;

            // Get logger
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("SendMail Application Starting...");

            SendMail(args, serviceProvider, logger);
        }

        private static string[] ParseCommaSeparatedArgs(string[] args)
        {
            if (args.Length == 1 && args[0].Contains(','))
            {
                return args[0].Split(',');
            }
            return args;
        }


        private static void SendMail(string[] args, IServiceProvider serviceProvider, ILogger<Program> logger)
        {
            // Help for sending Mail
            if (args.Length >= 1 && args[0].ToLower() == "--help")
            {
                Console.WriteLine("Help for Mail Sending Command:");
                Console.WriteLine("[email address] [subject] [body]");
                Console.WriteLine("email address: The recipient's email address.");
                Console.WriteLine("subject: The subject line of the email.");
                Console.WriteLine("body: The main content of the email.");
                Console.WriteLine("Example:");
                Console.WriteLine("\"example@example.com\" \"Meeting Reminder\" \"Reminder: Meeting at 3 PM Today\"");
                return;
            }

            var mailSender = serviceProvider.GetRequiredService<MailService>();
            string recipient = args[0];
            string subject = args.Length > 1 ? args[1] : "";
            string body = args.Length > 2 ? args[2] : "";
            string attachmentPath = args.Length > 3 ? args[3] : null;

            mailSender.SendEmailUsingOutlook(recipient, subject, body, attachmentPath);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services, context.Configuration, context.HostingEnvironment);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddLogging(configure => configure.AddConsole())
                    .AddTransient<MailService>()
                    .AddSingleton<IConfiguration>(configuration);
        }
    }
}
