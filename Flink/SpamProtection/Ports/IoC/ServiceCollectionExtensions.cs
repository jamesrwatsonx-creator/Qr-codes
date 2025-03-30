using Flink.SpamProtection.Ports.Repositories;
using Flink.SpamProtection.Repositories;
using Flink.SpamProtection.Services;

namespace Flink.SpamProtection.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterSpamProtection(this IServiceCollection services) {
    services.AddHttpClient<EasyListSyncService>();
    return services
      .AddSingleton<IForbiddenDomainRepository, ForbiddenDomainRepository>()
      .AddHostedService<EasyListSyncService>();
  }
}
