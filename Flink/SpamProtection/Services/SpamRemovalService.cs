using CSharpFunctionalExtensions;
using Flink.ShortLinks.Ports;
using Flink.SpamProtection.Ports.Repositories;

namespace Flink.SpamProtection.Services;

public class SpamRemovalService(
  ILogger<SpamRemovalService> logger,
  IForbiddenDomainRepository forbiddenDomains,
  IServiceScopeFactory serviceScopeFactory) : BackgroundService {
  protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
    await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);

    logger.LogInformation($"Starting {nameof(SpamRemovalService)}");
    while (!cancellationToken.IsCancellationRequested) {
      await RunOnce();
      await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
    }
  }

  public async Task RunOnce() {
    try {
      using var scope = serviceScopeFactory.CreateScope();
      var shortLinkRepository = scope.ServiceProvider.GetRequiredService<IShortLinkRepository>();

      var shortLinkSnapshot = await shortLinkRepository.GetAll();
      if (shortLinkSnapshot.IsFailure) return;

      var removalTasks = shortLinkSnapshot.Value
        .Where(l => forbiddenDomains.ContainsDomain(new Uri(l.Uri).Host))
        .ToList()
        .Select(s => shortLinkRepository
          .DeleteAsync(s.Id)
          .Tap(() => logger.LogInformation("Removed {ShortLinkId} redirecting to {ShortLinkUrl}", s.Id, s.Uri)));

      await Task.WhenAll(removalTasks);
    } catch (Exception exc) {
      logger.LogError("Exception while cleaning up spam because {Error}", exc.Message);
    }
  }
}
