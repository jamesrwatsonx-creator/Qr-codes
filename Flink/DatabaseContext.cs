using System.Data;
using Flink.Domain;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Flink;

public class DatabaseContext(ILogger<DatabaseContext> logger) {
  public IDbConnection CreateConnection() {
    logger.LogDebug("Open Connection to Database");
    var dbConnection = new SqliteConnection("Data Source=flink.sqlite");
    dbConnection.Open();
    return dbConnection;
  }

  public async Task Initialize() {
    
    using var connection = CreateConnection();
    const string CreateShortenedLinkTable = $"""
                                             CREATE TABLE IF NOT EXISTS
                                             {nameof(ShortenedLink)} (
                                               {nameof(ShortenedLink.Id)}             TEXT        NOT NULL        PRIMARY KEY,
                                               {nameof(ShortenedLink.Creator)}        TEXT,
                                               {nameof(ShortenedLink.Uri)}            TEXT,
                                               {nameof(ShortenedLink.CreationDate)}   TEXT
                                             );
                                             """;
    await connection.ExecuteAsync(CreateShortenedLinkTable);
  }
}
