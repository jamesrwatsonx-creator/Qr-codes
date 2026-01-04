using Flink.Events.Ports;
using Flink.ShortLinks.Ports;
using Flink.SpamProtection.BusinessLogic;
using Flink.SpamProtection.Ports.Repositories;

namespace Flink.SpamProtection.Services;

public class SpamRemovalService(
  IEventNotificationQueue eventNotificationQueue,
  ILogger<SpamRemovalService> logger,
  IBlockedHostRepository blockedHosts,
  IServiceScopeFactory serviceScopeFactory) : BackgroundService {
  protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
    await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);

    logger.LogInformation($"Starting {nameof(SpamRemovalService)}");
    while (!cancellationToken.IsCancellationRequested) {
      try {
        using var scope = serviceScopeFactory.CreateScope();
        var shortLinkRepository = scope.ServiceProvider.GetRequiredService<IShortLinkRepository>();
        await SpamRemovalBl.HandleBlockedLinkRemovalAsync(shortLinkRepository, blockedHosts, eventNotificationQueue);
      } catch (Exception exc) {
        logger.LogError("Exception while cleaning up spam because {Error}", exc.Message);
      }
      await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
    }
  }
}
