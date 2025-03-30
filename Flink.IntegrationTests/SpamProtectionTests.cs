using System.Net;
using Flink.IntegrationTests.TestScaffolding;
using FluentAssertions;

namespace Flink.IntegrationTests;

public class SpamProtectionTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory>  {
  [Theory]
  [InlineData("/spam-protection")]
  [InlineData("/spam-protection/sources")]
  public async Task SpamProtectionSiteRequested_NoParametersPassed_Returns200(string url) {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync(url);

    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
