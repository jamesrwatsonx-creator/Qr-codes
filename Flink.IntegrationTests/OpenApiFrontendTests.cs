using System.Net;
using Flink.IntegrationTests.TestScaffolding;
using FluentAssertions;

namespace Flink.IntegrationTests;

public class OpenApiFrontendTests {
  [Theory]
  [InlineData("/scalar")]
  [InlineData("/scalar/v1")]
  [InlineData("/swagger/")]
  [InlineData("/openapi/v1.json")]
  [InlineData("/swagger/index.html")]
  public async Task FlinkIsUp_HttpRequestToOpenApiClientEndpoint_Returns200OK(string endpoint) {
    var client = new FlinkWebApplicationFactory().CreateClient();
    var response = await client.GetAsync(endpoint);
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
