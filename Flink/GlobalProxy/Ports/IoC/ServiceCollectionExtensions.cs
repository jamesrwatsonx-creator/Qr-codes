using Microsoft.Extensions.Http;

namespace Flink.GlobalProxy.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterGlobalWebProxy(this IServiceCollection serviceCollection) {
    return serviceCollection.AddSingleton<IHttpMessageHandlerBuilderFilter, GlobalProxyHandlerBuilderFilter>();
  }
}
