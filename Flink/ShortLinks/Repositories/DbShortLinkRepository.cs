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
      return Result.Failure<ShortenedLink>($"Could not insert {shortenedLink.Id} into database");
    }
  }

  public async Task<Result<Maybe<ShortenedLink>>> GetAsync(string shortLinkId) {
    try {
      var result = await dbContext.ShortenedLinks.FindAsync(shortLinkId);
      return Result.Success(result == null ? Maybe<ShortenedLink>.None : Maybe.From(result));
    } catch {
      return Result.Failure<Maybe<ShortenedLink>>($"Could not query {shortLinkId} from database");
    }
  }

  public async Task<Result<List<ShortenedLink>>> GetAll() {
    try {
      return Result.Success(await dbContext.ShortenedLinks.ToListAsync());
    } catch {
      return Result.Failure<List<ShortenedLink>>("Could not query from database");
    }
  }

  public async Task<Result> DeleteAsync(string shortLinkId) {
    try {
      var entity = await dbContext.ShortenedLinks.FindAsync(shortLinkId);
      if (entity == null) return Result.Success();
      dbContext.ShortenedLinks.Remove(entity);
      await dbContext.SaveChangesAsync();
      return Result.Success();
    } catch {
      return Result.Failure("Could not delete from database");
    }
  }
}
