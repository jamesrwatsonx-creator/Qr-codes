using CSharpFunctionalExtensions;
using Dapper;
using Flink.Domain;

namespace Flink.MetaData.Repositories;

public class SqliteMetaDataRepository(DatabaseContext dbContext) : IMetaDataRepository {
  private const string AddOrUpdateMetaDataStatement = $"""
                                                       INSERT OR REPLACE INTO {nameof(ShortLinkMetaData)}
                                                         ({nameof(ShortLinkMetaData.ShortLinkId)},
                                                          {nameof(ShortLinkMetaData.Title)},
                                                          {nameof(ShortLinkMetaData.Author)},
                                                          {nameof(ShortLinkMetaData.Description)},
                                                          {nameof(ShortLinkMetaData.Image)},
                                                          {nameof(ShortLinkMetaData.LastUpdate)})
                                                       VALUES
                                                         (@{nameof(ShortLinkMetaData.ShortLinkId)},
                                                          @{nameof(ShortLinkMetaData.Title)},
                                                          @{nameof(ShortLinkMetaData.Author)},
                                                          @{nameof(ShortLinkMetaData.Description)},
                                                          @{nameof(ShortLinkMetaData.Image)},
                                                          @{nameof(ShortLinkMetaData.LastUpdate)})
                                                       """;

  public const string GetMetaDataByShortLinkIdQuery =
    $"SELECT * FROM {nameof(ShortLinkMetaData)} WHERE {nameof(ShortLinkMetaData.ShortLinkId)} = @Id";
  
  public async Task<Result<ShortLinkMetaData>> AddOrUpdateAsync(ShortLinkMetaData shortLinkMetaData) {
    try {
      using var connection = dbContext.CreateConnection();
      await connection.ExecuteAsync(AddOrUpdateMetaDataStatement, shortLinkMetaData);
      return Result.Success(shortLinkMetaData);
    } catch {
      return Result.Failure<ShortLinkMetaData>("Could not insert into Database");
    }
  }

  public async Task<Result<Maybe<ShortLinkMetaData>>> Get(string shortLinkId) {
    try {
      using var connection = dbContext.CreateConnection();
      var result = await connection.QueryFirstOrDefaultAsync<ShortLinkMetaData>(GetMetaDataByShortLinkIdQuery, new { Id = shortLinkId });
      return Result.Success(result == null ? Maybe<ShortLinkMetaData>.None : Maybe.From(result));
    } catch {
      return Result.Failure<Maybe<ShortLinkMetaData>>("Could not query from database");
    }
  }
}
