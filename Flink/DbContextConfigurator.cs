using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Flink;

public enum SupportedDatabases {
  Sqlite,
  Postgres,
  MySql,
}

public class DbConfiguration {
  [Required]
  [EnumDataType(typeof(SupportedDatabases))]
  public required SupportedDatabases DbType { get; init; }

  public string? PostgresConnectionString { get; init; } = null;
  public string? MySqlConnectionString { get; init; } = null;
  public string? SqliteConnectionString { get; init; } = null;
}

public static class DbContextConfigurator {
  public static void ConfigureDb(this DbContextOptionsBuilder builder, DbConfiguration dbConfiguration) {
    switch (dbConfiguration.DbType) {
      case SupportedDatabases.Sqlite:
        builder.UseSqlite(dbConfiguration.SqliteConnectionString);
        break;
      case SupportedDatabases.Postgres:
        builder.UseNpgsql(dbConfiguration.PostgresConnectionString);
        break;
      case SupportedDatabases.MySql:
        builder.UseMySql(dbConfiguration.MySqlConnectionString, ServerVersion.AutoDetect(dbConfiguration.MySqlConnectionString));
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(dbConfiguration.DbType), dbConfiguration.DbType, null) {
          HelpLink = null,
          HResult = 0,
          Source = null
        };
    }
  }
}
