using System.Net;

namespace Flink.Embed;

public static class StringExtensions {
  public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…") {
    return value?.Length > maxLength
      ? value[..maxLength] + truncationSuffix
      : value;
  }
  
  public static string? HtmlEncode(this string? value) {
    return WebUtility.HtmlEncode(value);
  }
}
