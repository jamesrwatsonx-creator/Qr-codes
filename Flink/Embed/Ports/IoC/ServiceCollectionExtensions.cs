namespace Flink.Embed.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterEmbedDomain(this IServiceCollection services) {
    return services.AddSingleton<EmbedMetricsRegistry>();
  }
}
