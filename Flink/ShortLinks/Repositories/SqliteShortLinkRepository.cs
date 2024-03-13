using CSharpFunctionalExtensions;
using Dapper;
using Flink.Domain;
using Flink.ShortLinks.Ports;

namespace Flink.ShortLinks.Repositories;

public class SqliteShortLinkRepository(DatabaseContext dbContext) : IShortLinkRepository {
  private const string InsertStatement = $"""
                                          INSERT INTO {nameof(ShortenedLink)} (
                                            {nameof(ShortenedLink.Id)},
                                            {nameof(ShortenedLink.Creator)},
                                            {nameof(ShortenedLink.Uri)},
                                            {nameof(ShortenedLink.CreationDate)}
                                          ) VALUES (
                                            @{nameof(ShortenedLink.Id)},
                                            @{nameof(ShortenedLink.Creator)},
                                            @{nameof(ShortenedLink.Uri)},
                                            @{nameof(ShortenedLink.CreationDate)}
                                          )
                                          """;

  private const string QueryAll = $"SELECT * FROM {nameof(ShortenedLink)}";
  private const string QueryByIdStatement = $"SELECT * FROM {nameof(ShortenedLink)} WHERE {nameof(ShortenedLink.Id)} = @Id";

  public async Task<Result<ShortenedLink>> AddAsync(ShortenedLink shortenedLink) {
    try {
      using var connection = dbContext.CreateConnection();
      await connection.ExecuteAsync(InsertStatement, shortenedLink);
      return Result.Success(shortenedLink);
    } catch {
      return Result.Failure<ShortenedLink>("Could not insert into Database");
    }
  }

  public async Task<Result<Maybe<ShortenedLink>>> GetAsync(string id) {
    try {
      using var connection = dbContext.CreateConnection();
      var result = await connection.QueryFirstOrDefaultAsync<ShortenedLink>(QueryByIdStatement, new { Id = id });
      return Result.Success(result == null ? Maybe<ShortenedLink>.None : Maybe.From(result));
    } catch {
      return Result.Failure<Maybe<ShortenedLink>>("Could not query from database");
    }
  }

  public async Task<Result<List<ShortenedLink>>> GetAll() {
    try {
      using var connection = dbContext.CreateConnection();
      var result = await connection.QueryAsync<ShortenedLink>(QueryAll);
      return Result.Success(result.ToList());
    } catch {
      return Result.Failure<List<ShortenedLink>>("Could not query from database");
    }
  }
}
