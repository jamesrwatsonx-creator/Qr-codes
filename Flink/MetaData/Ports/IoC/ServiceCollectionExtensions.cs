using Flink.MetaData.Database;
using Flink.MetaData.Repositories;
using Flink.MetaData.Services;

namespace Flink.MetaData.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterMetaDataDomain(this IServiceCollection serviceCollection, DbConfiguration dbOptions) {
    serviceCollection
      .AddHttpClient<IMetaDataScraper, MetaDataScraper>();
    
    serviceCollection
      .AddDbContext<MetaDataDbContext>(ctxOpts => ctxOpts.ConfigureDb(dbOptions));

    return serviceCollection
      .AddSingleton<ScrapingQueueMetricsRegistry>()
      .AddScoped<IMetaDataRepository, DbMetaDataRepository>()
      .AddSingleton<IScrapingQueue, InMemoryScrapingQueue>()
      .AddTransient<IMetaDataScraper, MetaDataScraper>()
      .AddHostedService<ScrapingBackgroundService>();
  }
}
