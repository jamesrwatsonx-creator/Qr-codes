using Flink.QRCodes.Repositories;

namespace Flink.QRCodes.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterQrCodeDomain(this IServiceCollection serviceCollection) {
    return serviceCollection
      .AddSingleton<QrCodeMetricsRegistry>()
      .AddSingleton<IQrCodeCache, QrCodeCache>();
  }
}
