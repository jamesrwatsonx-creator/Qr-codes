using Flink.QRCodes.Ports.IoC;
using Flink.QRCodes.Repositories;
using Flink.ShortLinks.Ports;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Flink.UnitTests.QrCodes;

public class IoCTests {
  [Fact]
  public void QrCodeDomainInjected_AllDependenciesAreProvided_QrCodeCacheShallBeRegistered() {
    var serviceCollection = new ServiceCollection();
    serviceCollection
      // dependencies of the domain
      .AddMemoryCache()
      .AddSingleton(Mock.Of<IShortLinkRepository>())
      // the domain itself
      .RegisterQrCodeDomain();

    serviceCollection.BuildServiceProvider()
      .GetService<IQrCodeCache>().Should().NotBeNull();
  }
}
