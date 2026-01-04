namespace Flink.SpamProtection.Ports.Models;

public sealed class SpamProtectionStatus {
  public bool Enabled { get; init; }
  public int BlockedHostsCount { get; set; }
  public int BlockListSourcesCount { get; set; }
}
