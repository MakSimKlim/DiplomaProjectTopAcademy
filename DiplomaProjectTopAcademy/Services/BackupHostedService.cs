using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using DiplomaProjectTopAcademy.Controllers;

namespace DiplomaProjectTopAcademy.Services
{
    public class BackupHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BackupHostedService> _logger;

        public BackupHostedService(IServiceProvider services, ILogger<BackupHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackupHostedService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = BackupController.GetDelay();
                if (delay == Timeout.InfiniteTimeSpan)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                await Task.Delay(delay, stoppingToken);
                if (stoppingToken.IsCancellationRequested) break;

                using var scope = _services.CreateScope();
                var controller = scope.ServiceProvider.GetRequiredService<BackupController>();

                try
                {
                    // имитируем нажатие кнопки
                    controller.CreateBackup();
                    _logger.LogInformation("Automatic backup created at {time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Automatic backup failed");
                }
            }

            _logger.LogInformation("BackupHostedService stopped.");
        }
    }
}
