using System.Net;
using System.Net.Http.Headers;
using Flink.IntegrationTests.TestScaffolding;
using Flink.Metrics.Ports.Models;
using FluentAssertions;

namespace Flink.IntegrationTests;

public class MetricsTests {
  [Fact]
  public async Task FlinkIsRunningMetricsActiveNoAuth_NoAuthCredentialsSupplied_MetricsAreExposed() {
    var appSettingOverrides = new Dictionary<string, string> {
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.EnableMetrics)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.RequireBasicAuth)}", "false" }
    };
    var r = await new FlinkWebApplicationFactory(appSettingOverrides)
      .CreateClient()
      .GetAsync("/metrics");

    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
  
  [Fact]
  public async Task FlinkIsRunningMetricsActiveNoAuth_RandomCredentialsSupplied_MetricsAreExposed() {
    var appSettingOverrides = new Dictionary<string, string> {
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.EnableMetrics)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.RequireBasicAuth)}", "false" }
    };
    var r = await new FlinkWebApplicationFactory(appSettingOverrides)
      .CreateClient()
      .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/metrics") {
        Headers = { Authorization = new AuthenticationHeaderValue("Basic", "Zjpm")}
      });
    
    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
  
  [Fact]
  public async Task FlinkIsRunningMetricsActiveAuthActive_CorrectCredentialsSupplied_MetricsAreExposed() {
    var appSettingOverrides = new Dictionary<string, string> {
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.EnableMetrics)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.RequireBasicAuth)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.BasicAuthPassword)}", "gulasch" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.BasicAuthUsername)}", "gulasch" },
    };
    var r = await new FlinkWebApplicationFactory(appSettingOverrides)
      .CreateClient()
      .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/metrics") {
        Headers = { Authorization = new AuthenticationHeaderValue("Basic", "Z3VsYXNjaDpndWxhc2No")}
      });
    
    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
  
  [Fact]
  public async Task FlinkIsRunningMetricsActiveAuthActive_WrongCredentialsSupplied_Unauthorized() {
    var appSettingOverrides = new Dictionary<string, string> {
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.EnableMetrics)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.RequireBasicAuth)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.BasicAuthPassword)}", "flnk" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.BasicAuthUsername)}", "flnk" },
    };
    var r = await new FlinkWebApplicationFactory(appSettingOverrides)
      .CreateClient()
      .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/metrics") {
        Headers = { Authorization = new AuthenticationHeaderValue("Basic", "Z3VsYXNjaDpndWxhc2No")}
      });
    
    r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }
  
  [Fact]
  public async Task FlinkIsRunningMetricsActiveAuthActive_NoCredentialsSupplied_Unauthorized() {
    var appSettingOverrides = new Dictionary<string, string> {
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.EnableMetrics)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.RequireBasicAuth)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.BasicAuthPassword)}", "flnk" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.BasicAuthUsername)}", "flnk" },
    };
    var r = await new FlinkWebApplicationFactory(appSettingOverrides)
      .CreateClient()
      .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/metrics"));
    
    r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }
  
  [Fact]
  public async Task FlinkIsRunningMetricsAreDisabled_NoAuthCredentialsSupplied_MetricsEndpointReturns404() {
    var appSettingOverrides = new Dictionary<string, string> {
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.EnableMetrics)}", "false" }
    };
    var r = await new FlinkWebApplicationFactory(appSettingOverrides)
      .CreateClient()
      .GetAsync("/metrics");

    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
  
  [Fact]
  public async Task FlinkIsRunningMetricsAreDisabled_WrongCredentialsSupplied_MetricsEndpointReturns404() {
    var appSettingOverrides = new Dictionary<string, string> {
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.EnableMetrics)}", "false" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.RequireBasicAuth)}", "true" }
    };
    
    var r = await new FlinkWebApplicationFactory(appSettingOverrides)
      .CreateClient()
      .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/metrics") {
        Headers = { Authorization = new AuthenticationHeaderValue("Basic", "Zjpm")}
      });

    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
  
  [Fact]
  public async Task FlinkIsRunningMetricsActiveNoAuthDifferentEndpoint_NoAuthCredentialsSupplied_MetricsAreExposed() {
    var appSettingOverrides = new Dictionary<string, string> {
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.EnableMetrics)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.RequireBasicAuth)}", "false" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.MetricsEndpointPath)}", "/herbert"}
    };
    var r = await new FlinkWebApplicationFactory(appSettingOverrides)
      .CreateClient()
      .GetAsync("/herbert");

    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
  
  [Fact]
  public async Task FlinkIsRunningMetricsActiveNoAuthDifferentEndpoint_NoAuthCredentialsSuppliedRequestToDefaultMetricsEndpoint_Returns404() {
    var appSettingOverrides = new Dictionary<string, string> {
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.EnableMetrics)}", "true" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.RequireBasicAuth)}", "false" },
      { $"{nameof(MetricsConfiguration)}:{nameof(MetricsConfiguration.MetricsEndpointPath)}", "/herbert"}
    };
    var r = await new FlinkWebApplicationFactory(appSettingOverrides)
      .CreateClient()
      .GetAsync("/metrics");

    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
}
