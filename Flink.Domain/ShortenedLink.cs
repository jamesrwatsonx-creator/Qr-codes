using System.ComponentModel.DataAnnotations;

namespace Flink.Domain;

public class ShortenedLink {
  [Key] public required string Id { get; init; }
  public required DateTime CreationDate { get; init; }
  public required string Creator { get; init; }
  public required string Uri { get; init; }
  public DateTime ActiveFrom { get; init; } = DateTime.MinValue.AddYears(1);
  public DateTime ActiveUntil { get; init; } = DateTime.MaxValue.AddYears(-1);

  public string? PasswordHash { get; init; } = null;
}

