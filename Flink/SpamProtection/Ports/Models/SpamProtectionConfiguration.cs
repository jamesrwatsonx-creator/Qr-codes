using System.ComponentModel.DataAnnotations;

namespace Flink.SpamProtection.Ports.Models;

public class SpamProtectionConfiguration {
  public required bool Enabled { get; init; }
  public required string[] BlockLists { get; init; }
  public required string[] CustomBlockedDomains { get; init; } = [];
  [Range(1, 720)]
  public required int RefreshIntervalInHours { get; init; }
}
