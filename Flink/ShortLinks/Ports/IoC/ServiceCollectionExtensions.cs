using Flink.ShortLinks.Repositories;

namespace Flink.ShortLinks.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterShortLinksDomain(this IServiceCollection serviceCollection) {
    return serviceCollection
      .AddScoped<IShortLinkRepository, SqliteShortLinkRepository>()
      .AddSingleton<ShortLinkMetricsRegistry>();
  }
}
