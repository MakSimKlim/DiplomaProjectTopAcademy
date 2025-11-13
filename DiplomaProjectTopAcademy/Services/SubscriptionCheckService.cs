using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SubscriptionCheckService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionCheckService> _logger;
    private readonly TimeSpan _checkInterval;

    public SubscriptionCheckService(IServiceProvider serviceProvider,
                                    ILogger<SubscriptionCheckService> logger,
                                    IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        int intervalMinutes = config.GetValue<int>("SubscriptionCheckIntervalMinutes", 1);
        _checkInterval = TimeSpan.FromMinutes(intervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var users = userManager.Users.ToList();

                foreach (var user in users)
                {
                    if (user.SubscriptionEndDate.HasValue && user.SubscriptionEndDate < DateTime.UtcNow)
                    {
                        if (user.IsActive)
                        {
                            user.IsActive = false;
                            await userManager.UpdateAsync(user);
                            _logger.LogInformation($"User {user.UserName} деактивирован — подписка истекла.");
                        }
                    }
                }
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}
