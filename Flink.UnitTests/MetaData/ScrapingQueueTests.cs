using Flink.Domain;
using Flink.MetaData;
using Flink.MetaData.Ports;
using Flink.UnitTests.TestSupport;
using FluentAssertions;

namespace Flink.UnitTests.MetaData;

public class ScrapingQueueTests {
  [Fact]
  public void EmptyQueue_EntryAdded_EnqueueResultIsSuccess() {
    // Arrange
    var shortenedLink = new ShortenedLink
      { CreationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Creator = "ruffy", Id = "h3LL0", Uri = "https://blog.rtrace.io" };
    var scrapingQueue = new InMemoryScrapingQueue(new ScrapingQueueMetricsRegistry(new UnitTestMeterFactory()));

    // Act
    var enqueueResult = scrapingQueue.EnqueueForScraping(shortenedLink);

    // Assert
    enqueueResult.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void QueueWithSingleElement_MaxCountDequeue_ReturnsSingleElement() {
    // Arrange
    var shortenedLink = new ShortenedLink
      { CreationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Creator = "ruffy", Id = "h3LL0", Uri = "https://blog.rtrace.io" };
    var scrapingQueue = new InMemoryScrapingQueue(new ScrapingQueueMetricsRegistry(new UnitTestMeterFactory()));
    scrapingQueue.EnqueueForScraping(shortenedLink);

    // Act
    var resultList = scrapingQueue.Dequeue(int.MaxValue).ToList();

    // Assert
    resultList.Should().HaveCount(1);
    resultList.Should().ContainEquivalentOf(shortenedLink);
  }

  [Fact]
  public void QueueWith2Elements_DequeueSingleElement_ReturnsFirstElementOfQueue() {
    // Arrange
    var shortenedLink1 = new ShortenedLink
      { CreationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Creator = "ruffy", Id = "1st", Uri = "https://blog.rtrace.io" };
    var shortenedLink2 = new ShortenedLink
      { CreationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Creator = "ruffy", Id = "2nd", Uri = "https://blog.rtrace.io" };
    var scrapingQueue = new InMemoryScrapingQueue(new ScrapingQueueMetricsRegistry(new UnitTestMeterFactory()));
    scrapingQueue.EnqueueForScraping(shortenedLink1);
    scrapingQueue.EnqueueForScraping(shortenedLink2);

    // Act
    var resultList = scrapingQueue.Dequeue(1).ToList();

    // Assert
    resultList.Should().HaveCount(1);
    resultList.Should().ContainEquivalentOf(shortenedLink1);
  }

  [Fact]
  public void EmptyQueue_AttemptToDequeue_ReturnsEmptyResult() {
    // Arrange
    var scrapingQueue = new InMemoryScrapingQueue(new ScrapingQueueMetricsRegistry(new UnitTestMeterFactory()));

    // Act
    var resultList = scrapingQueue.Dequeue(int.MaxValue).ToList();

    // Assert
    resultList.Should().BeEmpty();
  }
}
