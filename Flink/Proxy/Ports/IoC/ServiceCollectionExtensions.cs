using Flink.Shared.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Flink.Proxy.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterProxyDomain(this IServiceCollection serviceCollection) {
    serviceCollection
      .AddHttpClient<IForwardProxy, ForwardProxy>(c => c.AddBrowserImitationRequestHeaders());

    serviceCollection
      .AddSingleton<ForwardProxy>()
      .AddSingleton<IForwardProxy>(sp => {
        var proxied = sp.GetRequiredService<ForwardProxy>();
        var memoryCache = sp.GetRequiredService<IMemoryCache>();
        return new CachingForwardProxy(proxied, memoryCache);
      });

    return serviceCollection;
  }
}
