namespace Flink.WebUI.Ports.Models;

public class WebUiConfiguration {
  public required string DefaultTheme { get; init; } = "flink-default";
  public required int ThemeCacheDuration { get; init; } = 300;
}
