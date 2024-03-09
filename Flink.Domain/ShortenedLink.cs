namespace Flink.Domain;

public class ShortenedLink {
  public required string Id { get; set; }
  public required DateTime CreationDate { get; set; }
  public required string Creator { get; set; }
  public required string Uri { get; set; }
}
