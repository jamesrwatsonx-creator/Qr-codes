using CSharpFunctionalExtensions;
using Flink.Domain;

namespace Flink.MetaData;

public interface IMetaDataScraper {
  public Task<Result<ShortLinkMetaData>> ScrapeAsync(ShortenedLink shortenedLink);
}
