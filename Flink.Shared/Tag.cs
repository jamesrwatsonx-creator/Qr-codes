namespace Flink.Shared;

public static class Tag {
  public static KeyValuePair<string, object?> Create(string k, object? v) {
    return new KeyValuePair<string, object?>(k, v);
  }
}
