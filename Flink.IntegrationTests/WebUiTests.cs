using System.Net;
using Flink.IntegrationTests.TestScaffolding;
using FluentAssertions;

namespace Flink.IntegrationTests;

public class WebUiTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory> {
  private readonly List<string> _requiresHeaders = new[] {
    "X-Frame-Options",
    "Referrer-Policy",
    "X-Content-Type-Options",
    "Permissions-Policy",
    "Content-Security-Policy",
    "Cross-Origin-Embedder-Policy",
    "Cross-Origin-Opener-Policy",
    "Cross-Origin-Resource-Policy"
  }.ToList();

  [Theory]
  [InlineData("/")]
  [InlineData("/stats")]
  [InlineData("/pw/123")]
  public async Task WebUiRequest_NoParametersPassed_Returns200(string url) {
    // Act
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync(url);

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Theory]
  [InlineData("/")]
  [InlineData("/stats")]
  [InlineData("/pw/123")]
  public async Task WebUiRequest_NoParametersPassed_ReturnsSecurityHeaders(string url) {
    // Act
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync(url);

    // Assert
    _requiresHeaders.ForEach(h => r.Headers.Should().ContainKey(h));
  }
}
