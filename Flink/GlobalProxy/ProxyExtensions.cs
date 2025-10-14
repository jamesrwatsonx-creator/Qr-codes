namespace Flink.GlobalProxy;

public static class ProxyExtensions {
  public static string? EmptyToNull(this string? value) {
    var preppedValue = value?.Trim();
    return string.IsNullOrWhiteSpace(preppedValue) ? null : preppedValue;
  }

  public static string[] ParseCollection(this string? inputString) => string.IsNullOrWhiteSpace(inputString)
    ? []
    : inputString.Split(';', ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
