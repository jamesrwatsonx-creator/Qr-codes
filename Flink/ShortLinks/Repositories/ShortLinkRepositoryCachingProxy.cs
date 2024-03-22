using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.ShortLinks.Ports;
using Microsoft.Extensions.Caching.Memory;

namespace Flink.ShortLinks.Repositories;

public class ShortLinkRepositoryCachingProxy(IMemoryCache cache, IShortLinkRepository proxiedRepo) : IShortLinkRepository {
  public Task<Result<ShortenedLink>> AddAsync(ShortenedLink shortenedLink) => proxiedRepo
    .AddAsync(shortenedLink)
    .Tap(_ => cache.Set(shortenedLink.Id, shortenedLink, DateTimeOffset.UtcNow.AddHours(24)));

  public Task<Result<Maybe<ShortenedLink>>> GetAsync(string id) {
    if (cache.TryGetValue(id, out ShortenedLink? cachedObject) && cachedObject != null)
      return Task.FromResult(Result.Success(Maybe<ShortenedLink>.From(cachedObject)));
    return proxiedRepo
      .GetAsync(id)
      .Tap(shortenedLink => cache.Set(id, shortenedLink, DateTimeOffset.UtcNow.AddHours(24)));
  }

  public Task<Result<List<ShortenedLink>>> GetAll() => proxiedRepo.GetAll();
}
