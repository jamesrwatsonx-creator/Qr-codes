using System.Net;
using Flink.IntegrationTests.TestScaffolding;
using FluentAssertions;

namespace Flink.IntegrationTests;

public class WebUiTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory> {
  [Theory]
  [InlineData("/")]
  [InlineData("/stats")]
  public async Task IndexPageRequest_NoParametersPassed_Returns200(string url) {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync(url);

    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
