using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.Migrations;

namespace Flink.MetaData.Repositories;

public class DbMetaDataRepository(FlinkDbContext dbContext) : IMetaDataRepository {
  public async Task<Result<ShortLinkMetaData>> AddOrUpdateAsync(ShortLinkMetaData shortLinkMetaData) {
    try {
      var existingMetaData = await dbContext.ShortLinkMetaData.FindAsync(shortLinkMetaData.Id);
      if (existingMetaData == null)
        await dbContext.ShortLinkMetaData.AddAsync(shortLinkMetaData);
      else
        dbContext.Entry(existingMetaData).CurrentValues.SetValues(shortLinkMetaData);
      await dbContext.SaveChangesAsync();
      return Result.Success(shortLinkMetaData);
    } catch (Exception ex) {
      return Result.Failure<ShortLinkMetaData>("Could not insert into Database" + ex.Message);
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
