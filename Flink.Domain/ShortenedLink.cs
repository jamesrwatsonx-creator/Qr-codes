using System.ComponentModel.DataAnnotations;

namespace Flink.Domain;

public class ShortenedLink {
  [Key] public required string Id { get; init; }
  public required DateTime CreationDate { get; init; } = DateTime.UtcNow;
  public required string Creator { get; init; }
  public required string Uri { get; init; }
  public DateTime ActiveFrom { get; init; } = DateTime.SpecifyKind(DateTime.MinValue.AddYears(1), DateTimeKind.Utc);
  public DateTime ActiveUntil { get; init; } = DateTime.SpecifyKind(DateTime.MaxValue.AddYears(-1), DateTimeKind.Utc);
  public string? PasswordHash { get; init; } = null;
}
