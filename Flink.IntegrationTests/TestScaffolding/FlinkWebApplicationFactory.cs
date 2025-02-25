using Flink.Metrics.Ports.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flink.IntegrationTests.TestScaffolding;

// ReSharper disable once ClassNeverInstantiated.Global
public class FlinkWebApplicationFactory(Dictionary<string, string>? appSettingsOverrides = null) : WebApplicationFactory<Program> {
  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    builder.UseEnvironment("Flink.IntegrationTests");
    
    builder.ConfigureAppConfiguration((ctx, cfg) => {
      cfg.AddInMemoryCollection(appSettingsOverrides!);
    });
    
    builder.ConfigureServices(services => {
      var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(appSettingsOverrides!)
        .Build();
      services.AddSingleton<IOptions<MetricsConfiguration>>(_ => Options.Create(configuration.GetSection(nameof(MetricsConfiguration)).Get<MetricsConfiguration>()!));
    });
  }
}
