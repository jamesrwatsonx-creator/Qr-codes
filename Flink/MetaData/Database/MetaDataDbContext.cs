using Flink.Domain;
using Microsoft.EntityFrameworkCore;

namespace Flink.MetaData.Database;

public class MetaDataDbContext(DbContextOptions<MetaDataDbContext> contextOptions) : DbContext(contextOptions) {
  public DbSet<ShortLinkMetaData> ShortLinkMetaData { get; set; }
}
