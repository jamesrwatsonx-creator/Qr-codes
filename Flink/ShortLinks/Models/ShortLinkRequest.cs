using System.ComponentModel.DataAnnotations;

namespace Flink.ShortLinks.Models;

public class ShortLinkRequest {
  /// <summary>
  /// The URI (including HTTP, HTTPS Scheme as prefix) to shorten
  /// </summary>
  [Required] [Url] public required string Uri { get; init; }
  /// <summary>
  /// An ideally unique identifier of an entity/identity creating the shortened link
  /// </summary>
  [Required] public required string Creator { get; init; }

  /// <summary>
  /// In case a preferred Id is provided, Flink will attempt to use it as the Id
  /// </summary>
  public string? PreferredId { get; init; }

  /// <summary>
  /// In case no preferred Id is provided, Flink will create a random Id with the given length
  /// </summary>
  [Range(4, 100)]
  public int PreferredIdLength { get; init; } = 8;
}
