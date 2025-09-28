using System.ComponentModel.DataAnnotations;

namespace Flink.Events.Ports.Models;

public class EventNotificationSinkConfiguration {
  [Required] public required string Name { get; init; }

  [Required] [DataType(DataType.Url)] public required string Url { get; init; }

  [Required]
  [EnumDataType(typeof(HttpMethod))]
  public required string Method { get; init; } = "POST";

  [Required] public required string PayloadFormat { get; init; }
  public required FlinkEventNotificationType[] Events { get; init; } = [];
}
