using Flink.Shared.Extensions;

namespace Flink;

public static class ConfigurationExtensions {
  public static WebApplicationBuilder ValidateAndConfigure<T>(this WebApplicationBuilder builder, out T config) where T : class {
    var configSection = builder.Configuration
      .GetSection(typeof(T).Name);

    config = configSection
      .Get<T>()
      .ValidateConfiguration();

    builder.Services.Configure<T>(configSection);
    return builder;
  }

  public static WebApplicationBuilder ValidateAndConfigureFromRoot<T>(this WebApplicationBuilder builder, out T? config) where T : class {
    config = builder.Configuration
      .Get<T>()
      .ValidateConfiguration();
    
    builder.Services.Configure<T>(builder.Configuration);
    return builder;
  }

  public static WebApplicationBuilder ValidateAndConfigure<T>(this WebApplicationBuilder builder) where T : class {
    return builder.ValidateAndConfigure<T>(out _);
  }
}
