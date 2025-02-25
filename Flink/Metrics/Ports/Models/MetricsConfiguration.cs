namespace Flink.Metrics.Ports.Models;

public class MetricsConfiguration {
  public required bool EnableMetrics { get; init; } = true;
  public required bool RequireBasicAuth { get; init; } = true;
  public required string BasicAuthUsername { get; init; } = "flink";
  public required string BasicAuthPassword { get; init; } = "flink";
}
