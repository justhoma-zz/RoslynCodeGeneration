using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RoslynCodeGeneration
{
    internal class Program
    {
        internal static void Main(string[] args) =>
            CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    _ = config
                        .AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    _ = services
                        .AddOptions()
                        .Configure<CodeGenOptions>(hostContext.Configuration.GetSection(nameof(CodeGenOptions)))
                        .AddLogging()
                        .AddHostedService<ConsoleHostedService>()
                        .AddSingleton<CodeGenerationSample>();
                });
    }
}
