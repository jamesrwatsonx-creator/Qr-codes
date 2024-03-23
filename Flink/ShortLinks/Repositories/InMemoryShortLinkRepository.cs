using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.ShortLinks.Ports;

namespace Flink.ShortLinks.Repositories;

public class InMemoryShortLinkRepository : IShortLinkRepository {
  private readonly List<ShortenedLink> _memoryStorage = [];

  public Task<Result<ShortenedLink>> AddAsync(ShortenedLink shortenedLink) {
    _memoryStorage.Add(shortenedLink);
    return Task.FromResult(Result.Success(shortenedLink));
  }

  public Task<Result<Maybe<ShortenedLink>>> GetAsync(string id) {
    var elem = _memoryStorage.FirstOrDefault(x => x.Id == id);
    return Task.FromResult(Result.Success(elem == null ? Maybe<ShortenedLink>.None : Maybe.From(elem)));
  }

  public Task<Result<List<ShortenedLink>>> GetAll() {
    return Task.FromResult(Result.Success(_memoryStorage.ToList()));
  }
}
