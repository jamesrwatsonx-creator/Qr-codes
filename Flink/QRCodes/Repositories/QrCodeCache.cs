using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Memory;

namespace Flink.QRCodes.Repositories;

public class QrCodeCache(IMemoryCache cache) : IQrCodeCache {
  public Result<byte[]> Cache(string id, int height, byte[] data) {
    cache.Set(BuildCacheKey(id, height), data, DateTimeOffset.UtcNow.AddHours(24));
    return Result.Success(data);
  }

  public Result<Maybe<byte[]>> Retrieve(string id, int height) {
    return cache.TryGetValue(BuildCacheKey(id, height), out byte[]? data) && data != null
      ? Result.Success<Maybe<byte[]>>(data)
      : Result.Success(Maybe<byte[]>.None);
  }

  private static string BuildCacheKey(string id, int height) {
    return $"{id}-{height}px";
  }
}
