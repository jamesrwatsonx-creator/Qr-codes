using Flink.ShortLinks.Ports.Rbac;
using Flink.ShortLinks.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Flink.ShortLinks.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterShortLinksDomain(this IServiceCollection serviceCollection) {
    serviceCollection.AddAuthorization(options => {
      options.AddPolicy("CanDeleteLink", policy => {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new CanDeleteShortenedLinkRequirement());
      });
    });

    serviceCollection.AddSingleton<IAuthorizationHandler, CanDeleteShortenedLinkRequirementHandler>();

    return serviceCollection
      .AddSingleton(TimeProvider.System)
      .AddScoped<DbShortLinkRepository>()
      .AddScoped<IShortLinkRepository>(provider => {
        var memoryCache = provider.GetRequiredService<IMemoryCache>();
        var proxiedRepo = provider.GetRequiredService<DbShortLinkRepository>();
        return new ShortLinkRepositoryCachingProxy(memoryCache, proxiedRepo);
      })
      .AddSingleton<ShortLinkMetricsRegistry>();
  }
}
