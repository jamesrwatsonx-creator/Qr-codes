namespace Flink.GlobalProxy;

public static class EmptyToNullStringExtensions {
  public static string? EmptyToNull(this string? value) {
    var preppedValue = value?.Trim();
    return string.IsNullOrWhiteSpace(preppedValue) ? null : preppedValue;
  }
}
