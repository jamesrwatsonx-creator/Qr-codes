using System.Net;
using Flink.SpamProtection.Ports.Repositories;
using Flink.SpamProtection.Repositories;
using Flink.SpamProtection.Services;

namespace Flink.SpamProtection.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterSpamProtection(this IServiceCollection services) {
    services
      .AddHttpClient<BlockListSyncService>()
      .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler {
        AllowAutoRedirect = true,
        UseCookies = false,
        AutomaticDecompression = DecompressionMethods.All
      });

    return services
      .AddSingleton<IBlockedHostRepository, BlockedHostInMemoryRepository>()
      .AddSingleton<IBlockListSourceRepository, BlockListSourceInMemoryRepository>()
      .AddHostedService<BlockListSyncService>()
      .AddHostedService<SpamRemovalService>();
  }
}
