namespace Flink.Domain;

public class ShortLinkMetaData {
  public required string ShortLinkId { get; init; }
  public string? Title { get; init; }
  public string? Description { get; init; }
  public string? Author { get; init; }
  public string? Image { get; init; }
  public DateTime LastUpdate { get; init; }
}
