using Flink.WebUI.Ports.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Flink.WebUI.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterWebUi(this IServiceCollection serviceCollection) {
    return serviceCollection
      .AddSingleton<ThemeManager>()
      .AddScoped<IThemeManager, InMemoryCachingThemeManager>(s => {
        var themeManager = s.GetRequiredService<ThemeManager>();
        var memoryCache = s.GetRequiredService<IMemoryCache>();
        var webUiConfig = s.GetRequiredService<IOptions<WebUiConfiguration>>();
        return new InMemoryCachingThemeManager(memoryCache, themeManager, webUiConfig);
      });
  }
}
