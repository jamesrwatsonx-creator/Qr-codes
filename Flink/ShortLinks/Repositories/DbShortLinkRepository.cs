using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.Migrations;
using Flink.ShortLinks.Ports;
using Microsoft.EntityFrameworkCore;

namespace Flink.ShortLinks.Repositories;

public class DbShortLinkRepository(FlinkDbContext dbContext) : IShortLinkRepository {
  public async Task<Result<ShortenedLink>> AddAsync(ShortenedLink shortenedLink) {
    try {
      await dbContext.ShortenedLinks.AddAsync(shortenedLink);
      await dbContext.SaveChangesAsync();
      return Result.Success(shortenedLink);
    } catch {
      return Result.Failure<ShortenedLink>("Could not insert into Database");
    }
  }

  public async Task<Result<Maybe<ShortenedLink>>> GetAsync(string id) {
    try {
      var result = await dbContext.ShortenedLinks.FindAsync(id);
      return Result.Success(result == null ? Maybe<ShortenedLink>.None : Maybe.From(result));
    } catch {
      return Result.Failure<Maybe<ShortenedLink>>("Could not query from database");
    }
  }

  public async Task<Result<List<ShortenedLink>>> GetAll() {
    try {
      return Result.Success(await dbContext.ShortenedLinks.ToListAsync());
    } catch {
      return Result.Failure<List<ShortenedLink>>("Could not query from database");
    }
  }
}
