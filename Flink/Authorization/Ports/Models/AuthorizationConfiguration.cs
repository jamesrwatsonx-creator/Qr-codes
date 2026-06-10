namespace Flink.Authorization.Ports.Models;

public class AuthorizationConfiguration {
  public required List<AuthorizedIdentity> PrivilegedIdentities { get; init; } = [];
  public List<AuthorizedIdentity> Moderators => PrivilegedIdentities.Where(r => r.Role == Role.Moderator).ToList();
  public List<AuthorizedIdentity> Administrators => PrivilegedIdentities.Where(r => r.Role == Role.Administrator).ToList();
}
