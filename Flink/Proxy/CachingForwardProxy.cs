using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Flink.Proxy;

public class CachingForwardProxy(IForwardProxy proxiedImplementation, IMemoryCache memoryCache) : IForwardProxy {
  public Task<Result<FileContentResult>> Proxy(string url) {
    if (memoryCache.TryGetValue(url, out FileContentResult? cachedResult) && cachedResult != null)
      return Task.FromResult(Result.Success(cachedResult));

    return proxiedImplementation.Proxy(url)
      .Ensure(fileContentResult => fileContentResult != null, "Proxy returned unexpected Null Response")
      .Tap(fileContentResult => memoryCache.Set(url, fileContentResult));
  }
}
