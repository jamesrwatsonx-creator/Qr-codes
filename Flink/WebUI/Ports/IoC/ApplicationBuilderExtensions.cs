namespace Flink.WebUI.Ports.IoC;

public static class ApplicationBuilderExtensions {
  public static IApplicationBuilder UseWebUi(this IApplicationBuilder appBuilder) {
    return appBuilder.UseStaticFiles();
  }
}
