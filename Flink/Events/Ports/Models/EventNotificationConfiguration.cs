namespace Flink.Events.Ports.Models;

public class EventNotificationConfiguration {
  public required List<EventNotificationSinkConfiguration> Sinks { get; init; } = [];
}
