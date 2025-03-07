using CSharpFunctionalExtensions;
using Flink.Domain;

namespace Flink.ShortLinks.Ports;

public interface IShortLinkRepository {
  public Task<Result<ShortenedLink>> AddAsync(ShortenedLink shortenedLink);
  public Task<Result<Maybe<ShortenedLink>>> GetAsync(string shortLinkId);
  public Task<Result<List<ShortenedLink>>> GetAll();
  public Task<Result> DeleteAsync(string shortLinkId);
}
