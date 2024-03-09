using System.ComponentModel.DataAnnotations;

namespace Flink.ShortLinks.Models;

public class ShortLinkRequest {
  [Required]
  [Url]
  public required string Uri { get; set; }
  [Required]
  public required string Creator { get; set; }
  
  /// <summary>
  /// In case a preferred Id is provided, Flink will attempt to use it as the Id
  /// </summary>
  public string? PreferredId { get; set; }
  
  /// <summary>
  /// In case no preferred Id is provided, Flink will create a random Id with the given length
  /// </summary>
  [Range(4, 100)]
  public int PreferredIdLength { get; set; } = 8;
}
