using Flink.MetaData.Ports;
using Flink.MetaData.Ports.IoC;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Flink.UnitTests.MetaData;

public class IocTests {
  [Fact]
  public void MetaDataDomainInjected_AllDependenciesAreProvided_ScrapingQueueMustBeRegistered() {
    var serviceCollection = new ServiceCollection();
    serviceCollection.RegisterMetaDataDomain();

    serviceCollection.BuildServiceProvider()
      .GetService<IScrapingQueue>().Should().NotBeNull();
  }
}
