using System.ComponentModel.DataAnnotations;

namespace Flink.Domain;

public class ShortenedLink {
  [Key] public required string Id { get; init; }
  public required long CreationDate { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
  public required string Creator { get; init; }
  public required string Uri { get; init; }
  public long ActiveFrom { get; init; }
  public long ActiveUntil { get; init; } = long.MaxValue;
  public string? PasswordHash { get; init; } = null;
}
