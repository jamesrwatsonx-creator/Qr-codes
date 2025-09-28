namespace Flink.Events.Ports;

public class EventNotification(FlinkEventNotificationType eventType) {
  public required string Title { get; init; }
  public required string Message { get; init; }
  public required string Emoji { get; init; } = "";
  public required string? ImageUrl { get; init; } = null;
  public required Dictionary<string, Uri> Urls { get; init; } = new();
  public FlinkEventNotificationType EventType { get; init; } = eventType;
}
