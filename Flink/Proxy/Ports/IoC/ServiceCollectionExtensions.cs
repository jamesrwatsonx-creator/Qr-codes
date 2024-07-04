using Microsoft.Extensions.Caching.Memory;

namespace Flink.Proxy.Ports.IoC;

public static class ServiceCollectionExtensions {
  private const string UserAgent = "Mozilla/5.0 (X11; Linux i686; rv:127.0) Gecko/20100101 Firefox/127.0";
  public static IServiceCollection RegisterProxyDomain(this IServiceCollection serviceCollection) {
    serviceCollection
      .AddHttpClient<ForwardProxy>(c => c.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent));

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
