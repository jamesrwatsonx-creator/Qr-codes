using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData;
using Flink.MetaData.Ports;
using Flink.MetaData.Repositories;
using Flink.MetaData.Services;
using Flink.UnitTests.TestSupport;
using Flink.UnitTests.TestSupport.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Flink.UnitTests.MetaData;
/*
public class ScrapingBackgroundServiceTests {
  [Fact]
  public async Task TestSingleCycleScrape() {
    // Arrange
    var shortenedLink = new ShortenedLink { CreationDate = DateTime.UtcNow, Creator = "ruffy", Id = "1st", Uri = "https://blog.rtrace.io" };
    var metaData = new ShortLinkMetaData { Author = "ruffy", Description = "d", Image = "i", LastUpdate = DateTime.Now, ShortLinkId = "1", Title = "t" };

    var scrapingMock = new Mock<IMetaDataScraper>();
    scrapingMock
      .Setup(x => x.ScrapeAsync(It.IsAny<ShortenedLink>()))
      .ReturnsAsync(Result.Success(metaData));

    var metaDataRepo = new Mock<IMetaDataRepository>();
    metaDataRepo
      .Setup(x => x.AddOrUpdateAsync(It.IsAny<ShortLinkMetaData>()))
      .ReturnsAsync(Result.Success(metaData));

    var mockServiceProviderFactory = new Mock<IServiceProviderFactory<IServiceCollection>>();
    
    mockServiceProviderFactory
      .Setup(factory => factory.CreateBuilder(It.IsAny<IServiceCollection>()))
      .Returns((IServiceCollection services) => services);

    mockServiceProviderFactory
      .Setup(factory => factory.CreateServiceProvider(It.IsAny<IServiceCollection>()))
      .Returns(new ServiceCollection().BuildServiceProvider());

    var scrapingQueue = new InMemoryScrapingQueue(new ScrapingQueueMetricsRegistry(new UnitTestMeterFactory()));
    var service = new ScrapingBackgroundService(NullLogger<ScrapingBackgroundService>.Instance, mockServiceProviderFactory.Object);
    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

    // Act
    scrapingQueue.EnqueueForScraping(shortenedLink);
    await Task.Run(() => service.StartAsync(cts.Token), cts.Token);
    await cts.Token.WaitUntilCanceled();

    // Assert
    metaDataRepo.Verify(x => x.AddOrUpdateAsync(metaData), Times.Once);
  }
}
*/
