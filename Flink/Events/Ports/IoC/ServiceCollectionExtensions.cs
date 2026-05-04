using System.Net;
using Flink.Events.Services;

namespace Flink.Events.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterEventsDomain(this IServiceCollection services) {
    services
      .AddHttpClient<InMemoryEventNotificationNotificationQueue>()
      .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler {
        AllowAutoRedirect = true,
        UseCookies = true,
        AutomaticDecompression = DecompressionMethods.All,
      });

    services.AddHostedService<EventDispatcherBackgroundService>();
    services.AddHostedService<FlinkBootEventBackgroundService>();
    return services.AddSingleton<IEventNotificationQueue, InMemoryEventNotificationNotificationQueue>();
  }
}
