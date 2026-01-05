namespace Flink.SpamProtection.Models;

public class BlockListSource {
  public required string Source { get; init; }
  public required string Title { get; init; }
  public required ulong? Version { get; init; }
  public required string Sha3 { get; init; }
  public required Dictionary<string, string> MetaData { get; init; }
}
