namespace Flink.Metrics.Ports;

public static class Tag {
  public static KeyValuePair<string, object?> Create(string k, object? v) => new(k, v);
}
