using System.ComponentModel.DataAnnotations;

namespace Flink.Domain;

public class ShortLinkMetaData {
  [Key]
  public required string Id { get; init; }
  public string? Title { get; init; }
  public string? Description { get; init; }
  public string? Author { get; init; }
  public string? Image { get; init; }
  public int HttpStatusCode { get; init; }
  public int DocumentSize { get; init; }
  public long LastUpdate { get; init; }
}
