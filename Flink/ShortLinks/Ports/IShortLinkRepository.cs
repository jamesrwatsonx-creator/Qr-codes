using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.ShortLinks.Models;

namespace Flink.ShortLinks.Ports;

public interface IShortLinkRepository {
  public Task<Result<ShortenedLink>> AddAsync(ShortenedLink shortenedLink);
  public Task<Result<Maybe<ShortenedLink>>> GetAsync(string id);
  public Task<Result<List<ShortenedLink>>> GetAll();
}
