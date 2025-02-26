using System.Net;
using Flink.IntegrationTests.TestScaffolding;
using FluentAssertions;

namespace Flink.IntegrationTests;

public class IndexTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory> {
  [Fact]
  public async Task IndexPageRequest_NoParametersPassed_Returns200() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync("/");

    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task EmbedRequest_NoParametersPassed_Returns404() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync("/embed");

    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
}
