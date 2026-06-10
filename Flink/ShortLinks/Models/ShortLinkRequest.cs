using System.ComponentModel.DataAnnotations;

namespace Flink.ShortLinks.Models;

public class ShortLinkRequest {
  /// <summary>
  ///   The URI (including HTTP, HTTPS Scheme as prefix) to shorten
  /// </summary>
  [Required]
  [Url]
  public required string Uri { get; init; }

  /// <summary>
  ///   In case a preferred Id is provided, Flink will attempt to use it as the Id
  /// </summary>
  [Length(2, 250)]
  [RegularExpression("^[a-zA-Z0-9._~-]*$")]
  public string? PreferredId { get; init; }

  /// <summary>
  ///   In case no preferred Id is provided, Flink will create a random Id with the given length
  /// </summary>
  [Range(4, 100)]
  public int PreferredIdLength { get; init; } = 8;

  /// <summary>
  ///   (optional) the point in time until the shortened link is active. After this point in time, the link cannot be visited
  /// </summary>
  [DataType(DataType.DateTime)]
  public DateTimeOffset? ActiveUntil { get; init; } = null;

  /// <summary>
  ///   (optional) the point in time from which on the shortened link becomes active. Before this point in time, the link cannot be visited
  /// </summary>
  [DataType(DataType.DateTime)]
  public DateTimeOffset? ActiveFrom { get; init; } = null;

  /// <summary>
  ///    (optional) a password (better, a hash of it - so the plain-text password remains at the client) that protects the shortened link
  /// </summary>
  public string? Password { get; init; } = null;
}
