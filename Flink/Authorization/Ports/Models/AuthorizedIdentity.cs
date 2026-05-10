namespace Flink.Authorization.Ports.Models;

public sealed class AuthorizedIdentity {
  public required Role Role { get; init; }
  public required string UserId { get; init; }
}
