namespace Flink.SpamProtection.Models;

public class EasyListSource {
  public required string Source { get; init; }
  public required string Title { get; init; }
  public required ulong Version { get; init; }
  public required DateTime LastModified { get; init; }
}
