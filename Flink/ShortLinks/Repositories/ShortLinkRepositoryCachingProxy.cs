using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.ShortLinks.Ports;
using Microsoft.Extensions.Caching.Memory;

namespace Flink.ShortLinks.Repositories;

public class ShortLinkRepositoryCachingProxy(IMemoryCache cache, IShortLinkRepository proxiedRepo) : IShortLinkRepository {
  public Task<Result<ShortenedLink>> AddAsync(ShortenedLink shortenedLink) {
    return proxiedRepo
      .AddAsync(shortenedLink)
      .Tap(_ => cache.Set(shortenedLink.Id, shortenedLink, DateTimeOffset.UtcNow.AddHours(24)));
  }

  public Task<Result<Maybe<ShortenedLink>>> GetAsync(string shortLinkId) {
    if (cache.TryGetValue(shortLinkId, out ShortenedLink? cachedObject) && cachedObject != null)
      return Task.FromResult(Result.Success(Maybe<ShortenedLink>.From(cachedObject)));
    return proxiedRepo
      .GetAsync(shortLinkId)
      .Tap(shortenedLink => cache.Set(shortLinkId, shortenedLink, DateTimeOffset.UtcNow.AddHours(24)));
  }

  public Task<Result<List<ShortenedLink>>> GetAll() {
    return proxiedRepo.GetAll();
  }

  public Task<Result> DeleteAsync(string shortLinkId) {
    cache.Remove(shortLinkId);
    return proxiedRepo.DeleteAsync(shortLinkId);
  }
}
