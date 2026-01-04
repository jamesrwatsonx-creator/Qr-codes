namespace Flink.SpamProtection.Ports.Models;

public sealed class HostBlockStatus {
  public required string Host { get; init; }
  public required bool Blocked { get; init; }
}
