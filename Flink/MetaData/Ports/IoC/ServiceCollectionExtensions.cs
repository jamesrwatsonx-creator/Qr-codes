using System.Net;
using Flink.Embed.Ports;
using Flink.MetaData.Repositories;
using Flink.MetaData.Services;
using Flink.Shared.Extensions;

namespace Flink.MetaData.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterMetaDataDomain(this IServiceCollection serviceCollection) {
    serviceCollection
      .AddHttpClient<IMetaDataScraper, MetaDataScraper>(c => c.AddBrowserImitationRequestHeaders())
      .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler {
        AllowAutoRedirect = true,
        UseCookies = true,
        AutomaticDecompression = DecompressionMethods.All,
        MaxAutomaticRedirections = 10,
      });
    return serviceCollection
      .AddSingleton<ScrapingQueueMetricsRegistry>()
      .AddSingleton<EmbedMetricsRegistry>()
      .AddScoped<IMetaDataRepository, DbMetaDataRepository>()
      .AddSingleton<IScrapingQueue, InMemoryScrapingQueue>()
      .AddTransient<IMetaDataScraper, MetaDataScraper>()
      .AddHostedService<ScrapingBackgroundService>();
  }
}
