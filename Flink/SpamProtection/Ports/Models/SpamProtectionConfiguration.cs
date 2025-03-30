namespace Flink.SpamProtection.Ports.Models;

public class SpamProtectionConfiguration {
  public required bool Enabled { get; init; }
  public required string[] BlockLists { get; init; }

  public required string[] CustomBlockedDomains { get; init; } = [];
  public required int RefreshIntervalInHours { get; init; }
}
