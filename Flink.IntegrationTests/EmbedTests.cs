using Flink.IntegrationTests.TestScaffolding;
using FluentAssertions;

namespace Flink.IntegrationTests;

public class EmbedTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory> {
  private readonly List<string> _requiresHeaders = new[] {
    "Referrer-Policy",
    "X-Content-Type-Options",
    "Permissions-Policy",
    "Content-Security-Policy",
    "Cross-Origin-Embedder-Policy",
    "Cross-Origin-Opener-Policy",
    "Cross-Origin-Resource-Policy"
  }.ToList();

  [Theory]
  [InlineData("/links/existing/embed")]
  [InlineData("/links/non-existing/embed")]
  public async Task EmbedRequest_NoParametersPassed_ReturnsSecurityHeaders(string url) {
    // Act
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync(url);

    // Assert
    _requiresHeaders.ForEach(h => r.Headers.Should().ContainKey(h));
  }
}
