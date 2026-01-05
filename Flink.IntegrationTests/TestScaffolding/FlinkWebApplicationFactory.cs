using Flink.Metrics.Ports.Models;
using Flink.SpamProtection.Ports.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flink.IntegrationTests.TestScaffolding;

// ReSharper disable once ClassNeverInstantiated.Global
public class FlinkWebApplicationFactory : WebApplicationFactory<Program> {
  private readonly Dictionary<string, string> _appSettingsOverrides;

  public FlinkWebApplicationFactory() {
    _appSettingsOverrides = new Dictionary<string, string>();
  }

  internal FlinkWebApplicationFactory(Dictionary<string, string>? appSettingsOverrides = null) {
    _appSettingsOverrides = appSettingsOverrides ?? new Dictionary<string, string>();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    builder.UseEnvironment("Flink.IntegrationTests");

    builder.ConfigureAppConfiguration((ctx, cfg) => {
      cfg.AddInMemoryCollection(_appSettingsOverrides!);
    });

    builder.ConfigureServices(services => {
      if (_appSettingsOverrides.Count == 0)
        return;

      var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(_appSettingsOverrides!)
        .Build();

      var spamProtectionConfiguration = Options.Create(new SpamProtectionConfiguration {
        Enabled = false,
        BlockListSources = [],
        CustomBlockedHosts = [],
        RefreshIntervalInHours = 0
      });

      services.AddSingleton<IOptions<MetricsConfiguration>>(_ =>
        Options.Create(configuration.GetSection(nameof(MetricsConfiguration)).Get<MetricsConfiguration>()!));
      services.AddSingleton<IOptions<SpamProtectionConfiguration>>(_ => spamProtectionConfiguration);
    });
  }
}
