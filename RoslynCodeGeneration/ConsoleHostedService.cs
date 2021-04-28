using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RoslynCodeGeneration
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly CodeGenerationSample _codeGenerationSample;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger _logger;

        public ConsoleHostedService(
            CodeGenerationSample codeGenerationSample,
            IHostApplicationLifetime appLifetime,
            ILogger<ConsoleHostedService> logger)
        {
            _codeGenerationSample = codeGenerationSample ?? throw new ArgumentNullException(nameof(codeGenerationSample));
            _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

            _ = _appLifetime.ApplicationStarted.Register(() =>
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _codeGenerationSample
                            .ExecuteAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
