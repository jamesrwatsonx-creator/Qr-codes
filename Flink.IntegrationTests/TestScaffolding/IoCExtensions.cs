using Microsoft.Extensions.DependencyInjection;

namespace Flink.IntegrationTests.TestScaffolding;

public static class IoCExtensions {
  public static IServiceCollection RemoveDbContext<TContext>(this IServiceCollection services) {
    var dbC = services.Single(s => s.ServiceType == typeof(TContext));
    services.Remove(dbC);
    return services;
  }
}
