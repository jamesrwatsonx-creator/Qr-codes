using System.ComponentModel.DataAnnotations;

namespace Flink.Metrics.Ports.Models;

public class MetricsConfiguration {
  public required bool EnableMetrics { get; init; } = true;
  public required bool RequireBasicAuth { get; init; } = true;

  [StringLength(199, MinimumLength = 3)] public required string BasicAuthUsername { get; init; } = "flink";

  [DataType(DataType.Password)]
  [StringLength(199, MinimumLength = 3)]
  public required string BasicAuthPassword { get; init; } = "flink";

  [DataType(DataType.Text)] public required string MetricsEndpointPath { get; init; } = "/metrics";
}
