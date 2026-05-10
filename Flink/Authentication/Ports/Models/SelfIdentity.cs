namespace Flink.Authentication.Ports.Models;

public class SelfIdentity {
  public required string UserId { get; init; }
  public string? Name { get; init; } = null;
}
