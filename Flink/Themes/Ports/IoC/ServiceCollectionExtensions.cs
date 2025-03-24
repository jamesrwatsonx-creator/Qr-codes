using Microsoft.Extensions.Caching.Memory;

namespace Flink.Themes.Ports.IoC;

public static class ServiceCollectionExtensions {
  public static IServiceCollection RegisterThemeManager(this IServiceCollection services) {
    return services
      .AddSingleton<ThemeManager>()
      .AddScoped<IThemeManager, CachingThemeManager>(s => {
        var themeManager = s.GetRequiredService<ThemeManager>();
        var memoryCache = s.GetRequiredService<IMemoryCache>();
        return new CachingThemeManager(memoryCache, themeManager);
      });
  }
}
