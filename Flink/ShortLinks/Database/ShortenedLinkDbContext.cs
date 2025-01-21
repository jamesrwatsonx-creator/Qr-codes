using Flink.Domain;
using Microsoft.EntityFrameworkCore;

namespace Flink.ShortLinks.Database;

public class ShortenedLinkDbContext(DbContextOptions<ShortenedLinkDbContext> options) : DbContext(options) {
  public DbSet<ShortenedLink> ShortenedLinks { get; set; }
}
