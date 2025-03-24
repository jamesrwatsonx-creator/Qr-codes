using Microsoft.Extensions.Options;

namespace Flink.Themes;

public class ThemeManager(IWebHostEnvironment webHostEnvironment, IOptions<ThemeConfiguration> themeConfiguration) : IThemeManager {
  public string[] AvailableThemes() => 
    Directory.GetDirectories($"{webHostEnvironment.WebRootPath}/themes/")
      .Select(x => x.Replace($"{webHostEnvironment.WebRootPath}/themes/", string.Empty))
      .ToArray();

  public string GetFile(string fileName, string? theme) {
    var actualTheme = AvailableThemes().Contains(theme) ? theme : themeConfiguration.Value.DefaultTheme;
    return File.ReadAllText($"{webHostEnvironment.WebRootPath}/themes/{actualTheme}/{fileName}");
  }
}
