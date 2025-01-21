using System.Net;
using Flink.IntegrationTests.TestScaffolding;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Flink.IntegrationTests;

public class MetricsTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory> {
  [Fact]
  public async Task FlinkIsRunning_DefaultConfigurationIsApplied_MetricsAreExposed() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync("/metrics");

    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
