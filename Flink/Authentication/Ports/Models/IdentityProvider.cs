namespace Flink.Authentication.Ports.Models;

public class IdentityProvider {
  public required string Id { get; init; }
  public required string DisplayName { get; init; }
  public required string LoginUrl { get; init; }
  public required string ImageUrl { get; init; }
}
