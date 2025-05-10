using Flink.Embed.Ports;
using Flink.MetaData.Repositories;
using Flink.MetaData.Services;

namespace Flink.MetaData.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterMetaDataDomain(this IServiceCollection serviceCollection) {
    serviceCollection
      .AddHttpClient<IMetaDataScraper, MetaDataScraper>();

    return serviceCollection
      .AddSingleton<ScrapingQueueMetricsRegistry>()
      .AddSingleton<EmbedMetricsRegistry>()
      .AddScoped<IMetaDataRepository, DbMetaDataRepository>()
      .AddSingleton<IScrapingQueue, InMemoryScrapingQueue>()
      .AddTransient<IMetaDataScraper, MetaDataScraper>()
      .AddHostedService<ScrapingBackgroundService>();
  }
}
