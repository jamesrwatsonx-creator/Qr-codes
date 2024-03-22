using Flink.ShortLinks.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Flink.ShortLinks.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterShortLinksDomain(this IServiceCollection serviceCollection) {
    return serviceCollection
      .AddScoped<SqliteShortLinkRepository>()
      .AddScoped<IShortLinkRepository>(provider => {
        var memoryCache = provider.GetRequiredService<IMemoryCache>();
        var proxiedRepo = provider.GetRequiredService<SqliteShortLinkRepository>();
        return new ShortLinkRepositoryCachingProxy(memoryCache, proxiedRepo);
      })
      .AddSingleton<ShortLinkMetricsRegistry>();
  }
}
