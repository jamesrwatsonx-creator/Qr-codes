using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Flink.IntegrationTests;

public class MetricsTests(WebApplicationFactory<Program> webApplicationFactory) : IClassFixture<WebApplicationFactory<Program>> {
  [Fact]
  public async Task FlinkIsRunning_DefaultConfigurationIsApplied_MetricsAreExposed() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync("/metrics");

    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
