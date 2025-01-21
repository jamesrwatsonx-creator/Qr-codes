using Flink.ShortLinks.Database;
using Flink.ShortLinks.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Flink.ShortLinks.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterShortLinksDomain(this IServiceCollection serviceCollection, DbConfiguration dbOptions) {
    serviceCollection
      .AddDbContext<ShortenedLinkDbContext>(ctxOpts => ctxOpts.ConfigureDb(dbOptions));
    
    return serviceCollection
      .AddScoped<DbShortLinkRepository>()
      .AddScoped<IShortLinkRepository>(provider => {
        var memoryCache = provider.GetRequiredService<IMemoryCache>();
        var proxiedRepo = provider.GetRequiredService<DbShortLinkRepository>();
        return new ShortLinkRepositoryCachingProxy(memoryCache, proxiedRepo);
      })
      .AddSingleton<ShortLinkMetricsRegistry>();
  }
}
