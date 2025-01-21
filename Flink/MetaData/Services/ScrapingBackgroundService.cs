using Flink.MetaData.Ports;
using Flink.MetaData.Repositories;
using Flink.Shared.Extensions;

namespace Flink.MetaData.Services;

public class ScrapingBackgroundService(ILogger<ScrapingBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
  : BackgroundService {
  protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
    try {
      while (!cancellationToken.IsCancellationRequested) {
        using var scope = serviceScopeFactory.CreateScope();
        var taskQueue = scope.ServiceProvider.GetRequiredService<IScrapingQueue>();
        var metaDataScraper = scope.ServiceProvider.GetRequiredService<IMetaDataScraper>();
        var metaDataRepository = scope.ServiceProvider.GetRequiredService<IMetaDataRepository>();

        logger.LogDebug($"Executing {nameof(ScrapingBackgroundService)} worker cycle");
        var scrapeResults = await taskQueue.Dequeue(5)
          .Select(metaDataScraper.ScrapeAsync)
          .AwaitAll();

        await scrapeResults
          .Where(shortLinkMetaData => shortLinkMetaData.IsSuccess)
          .Select(shortLinkMetaData => shortLinkMetaData.Value)
          .Select(metaDataRepository.AddOrUpdateAsync)
          .AwaitAll();

        if (scrapeResults.Length == 0) // prevent busy polling
          await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
      }
    } catch (TaskCanceledException) { }
  }
}
