using Flink.WebUI.Ports.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Flink.WebUI;

public class InMemoryCachingThemeManager(IMemoryCache memoryCache, IThemeManager proxiedThemeManager, IOptions<WebUiConfiguration> webUiConfig) : IThemeManager {
  public string[] AvailableThemes() {
    if (memoryCache.TryGetValue("::AvailableThemes", out string[]? cachedThemes))
      return cachedThemes!;

    var themes = proxiedThemeManager.AvailableThemes();
    memoryCache.Set("::AvailableThemes", themes, DateTimeOffset.Now.AddSeconds(webUiConfig.Value.ThemeCacheDuration));
    return themes;
  }

  public string GetFile(string fileName, string? theme) {
    var cacheKey = $"__{fileName}::{theme??"default"}__";
    if (memoryCache.TryGetValue(cacheKey, out string? cachedFile))
      return cachedFile!;

    var file = proxiedThemeManager.GetFile(fileName, theme);
    memoryCache.Set(cacheKey, file, DateTimeOffset.Now.AddSeconds(webUiConfig.Value.ThemeCacheDuration));
    return file;
  }
}
