using System.ComponentModel.DataAnnotations;

namespace Flink.SpamProtection.Ports.Models;

public sealed class SpamProtectionConfiguration {
  public required bool Enabled { get; init; }
  public required string[] BlockListSources { get; init; }
  public required string[] CustomBlockedHosts { get; init; } = [];
  [Range(1, 720)] public required int RefreshIntervalInHours { get; init; } = 6;
}
