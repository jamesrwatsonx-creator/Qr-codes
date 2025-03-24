using Microsoft.Extensions.Caching.Memory;

namespace Flink.Themes;

public class CachingThemeManager(IMemoryCache memoryCache, IThemeManager proxiedThemeManager) : IThemeManager {
  public string[] AvailableThemes() {
    if (memoryCache.TryGetValue("::AvailableThemes", out string[]? cachedThemes))
      return cachedThemes!;

    var themes = proxiedThemeManager.AvailableThemes();
    memoryCache.Set("::AvailableThemes", themes, DateTimeOffset.Now.AddMinutes(1));
    return themes;
  }

  public string GetFile(string fileName, string? theme) {
    var cacheKey = $"__{fileName}::{theme??"default"}__";
    if (memoryCache.TryGetValue(cacheKey, out string? cachedFile))
      return cachedFile!;

    var file = proxiedThemeManager.GetFile(fileName, theme);
    memoryCache.Set(cacheKey, file, DateTimeOffset.Now.AddMinutes(1));
    return file;
  }
}
