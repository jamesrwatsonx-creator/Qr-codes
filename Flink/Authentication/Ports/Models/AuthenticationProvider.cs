namespace Flink.Authentication.Ports.Models;

// ReSharper disable once ClassNeverInstantiated.Global

public enum AuthProviderType {
  OpenIdConnect,
  OAuth
}

public class AuthenticationProvider {
  public required AuthProviderType AuthProviderType { get; init; }
  public required bool IsEnabled { get; init; } = false;
  public required string Id { get; init; }
  public required string DisplayName { get; init; } 
  public string? IconUrl { get; init; }

  public required string ClientId { get; init; }
  public required string ClientSecret { get; init; }

  // OIDC
  public string? Authority { get; set; }
  public List<string>? Scopes { get; set; }

  // OAuth
  public string? AuthorizationEndpoint { get; set; }
  public string? TokenEndpoint { get; set; }
  public string? UserInfoEndpoint { get; set; }
}
