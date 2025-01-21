using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData.Database;

namespace Flink.MetaData.Repositories;

public class DbMetaDataRepository(MetaDataDbContext dbContext) : IMetaDataRepository {
  public async Task<Result<ShortLinkMetaData>> AddOrUpdateAsync(ShortLinkMetaData shortLinkMetaData) {
    try {
      var existingMetaData = await dbContext.ShortLinkMetaData.FindAsync(shortLinkMetaData.ShortLinkId);
      if(existingMetaData == null) 
        await dbContext.ShortLinkMetaData.AddAsync(shortLinkMetaData);
      else 
        dbContext.Entry(existingMetaData).CurrentValues.SetValues(shortLinkMetaData);
      await dbContext.SaveChangesAsync();
      return Result.Success(shortLinkMetaData);
    } catch {
      return Result.Failure<ShortLinkMetaData>("Could not insert into Database");
    }
  }

  public async Task<Result<Maybe<ShortLinkMetaData>>> GetAsync(string shortLinkId) {
    try {
      var result = await dbContext.ShortLinkMetaData.FindAsync(shortLinkId);
      return Result.Success(result == null ? Maybe<ShortLinkMetaData>.None : Maybe.From(result));
    } catch {
      return Result.Failure<Maybe<ShortLinkMetaData>>("Could not query from database");
    }
  }
}
