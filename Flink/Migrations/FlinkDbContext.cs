using Flink.Domain;
using Microsoft.EntityFrameworkCore;

namespace Flink.Migrations;

public class FlinkDbContext(DbContextOptions<FlinkDbContext> contextOptions) : DbContext(contextOptions) {
  public DbSet<ShortenedLink> ShortenedLinks { get; set; }
  public DbSet<ShortLinkMetaData> ShortLinkMetaData { get; set; }
}
