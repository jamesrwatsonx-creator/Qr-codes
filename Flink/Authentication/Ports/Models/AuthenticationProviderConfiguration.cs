namespace Flink.Authentication.Ports.Models;

public class AuthenticationProviderConfiguration {
  public required List<AuthenticationProvider> Providers { get; init; } = [];
}
