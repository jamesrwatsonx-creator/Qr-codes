using CSharpFunctionalExtensions;
using Flink.Domain;

namespace Flink.MetaData.Repositories;

public interface IMetaDataRepository {
  public Task<Result<ShortLinkMetaData>> AddOrUpdateAsync(ShortLinkMetaData shortLinkMetaData);
  public Task<Result<Maybe<ShortLinkMetaData>>> Get(string shortLinkId);
}
