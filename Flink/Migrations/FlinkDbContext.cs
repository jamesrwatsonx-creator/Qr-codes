using Microsoft.EntityFrameworkCore;

namespace Flink.Migrations;

public class FlinkDbContext(DbContextOptions<FlinkDbContext> contextOptions) : DbContext(contextOptions) {
  public DbSet<Domain.ShortenedLink> ShortenedLinks { get; set; }
  public DbSet<Domain.ShortLinkMetaData> ShortLinkMetaData { get; set; }
}
