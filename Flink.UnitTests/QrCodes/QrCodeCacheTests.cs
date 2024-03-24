using Flink.QRCodes.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Flink.UnitTests.QrCodes;

public class QrCodeCacheTests {
  [Fact]
  public void QrCodeAddedToCache_ElementNotYetCached_CachesQrCode() {
    var data = Enumerable.Range(0, 512).Select(_ => (byte)0x1).ToArray();
    var qrCodeCache = new QrCodeCache(new MemoryCache(Options.Create(new MemoryCacheOptions())));
    var cacheResult = qrCodeCache.Cache(Guid.NewGuid().ToString(), 512, data);
    cacheResult.IsSuccess.Should().BeTrue();
    cacheResult.Value.SequenceEqual(data).Should().BeTrue();
  }
  
  [Fact]
  public void QrCodeQueriedFromCache_ElementNotYetCached_SucceedsButIsEmpty() {
    var qrCodeCache = new QrCodeCache(new MemoryCache(Options.Create(new MemoryCacheOptions())));
    var cacheResult = qrCodeCache.Retrieve($"{Guid.NewGuid()}-non-existing", 512);
    cacheResult.IsSuccess.Should().BeTrue();
    cacheResult.Value.HasNoValue.Should().BeTrue();
  }
  
  [Fact]
  public void QrCodeCodeHappyPath_ElementCachedAndQueried_ReturnsCachedQrCode() {
    var id = $"{Guid.NewGuid()}-actual-existing";
    var data = Enumerable.Range(0, 512).Select(_ => (byte)0x1).ToArray();
    var qrCodeCache = new QrCodeCache(new MemoryCache(Options.Create(new MemoryCacheOptions())));
    qrCodeCache.Cache(id,512, data);
    var cachedResult = qrCodeCache.Retrieve(id, 512);
    cachedResult.IsSuccess.Should().BeTrue();
    cachedResult.Value.HasValue.Should().BeTrue();
    cachedResult.Value.Value.SequenceEqual(data).Should().BeTrue();
  }
}
