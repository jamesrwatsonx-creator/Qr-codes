using Flink.Domain;
using Flink.ShortLinks.Ports;
using Flink.ShortLinks.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace Flink.UnitTests.ShortLinks;

public class ShortLinkRepositoryCachingProxyTests {
  
  [Fact]
  public async Task RequestShortLink_CachedEntryExists_ReturnsCachedEntryWithoutAskingRepository() {
    // Arrange
    var implMock = new Mock<IShortLinkRepository>();
    var shortLink = new ShortenedLink { Creator = "ruffy", CreationDate = DateTime.UtcNow, Id = "Gulasch", Uri = "https://gula.sh" };
    var cachingProxy = new ShortLinkRepositoryCachingProxy(new MemoryCache(Options.Create(new MemoryCacheOptions())), implMock.Object);
   
    var addResult = await cachingProxy.AddAsync(shortLink);
    addResult.IsSuccess.Should().BeTrue();
    
    // Act
    var getResult = await cachingProxy.GetAsync(shortLink.Id);
    
    // Assert
    getResult.IsSuccess.Should().BeTrue();
    getResult.Value.HasValue.Should().BeTrue();
    getResult.Value.Value.Should().BeEquivalentTo(shortLink);
    
    implMock.Verify(impl => impl.GetAsync(shortLink.Id), Times.Never);
  }
}
