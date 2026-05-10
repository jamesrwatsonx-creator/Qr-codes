using System.Net;
using System.Text;
using System.Text.Json;
using Flink.IntegrationTests.TestScaffolding;
using Flink.ShortLinks.Models;

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

  [Fact]
  public async Task PasswordProtectedEmbed_RequestedWithPassword_Returns200() {
    // Arrange
    var shortLinkId = $"{nameof(PasswordProtectedEmbed_RequestedWithPassword_Returns200)}-{Guid.NewGuid()}";
    var password = Guid.NewGuid().ToString();
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/",
      Password = password
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync("/links", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(4));
    var r = await httpClient.GetAsync($"/links/{shortLinkId}/embed?password={password}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task PasswordProtectedEmbed_RequestedWithoutPassword_ReturnsUnauthorizedEmbed() {
    // Arrange
    var shortLinkId = $"{nameof(PasswordProtectedEmbed_RequestedWithoutPassword_ReturnsUnauthorizedEmbed)}-{Guid.NewGuid()}";
    var password = Guid.NewGuid().ToString();
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/",
      Password = password
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync("/links", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(4));
    var r = await httpClient.GetAsync($"/links/{shortLinkId}/embed");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.OK);
    var body = await r.Content.ReadAsStringAsync();
    body.Should().Contain("Unauthorized");
  }

  [Fact]
  public async Task NonExistingEmbed_RequestedByUser_ReturnsNotFoundEmbed() {
    // Arrange
    var httpClient = webApplicationFactory.CreateClient();

    // Act
    var r = await httpClient.GetAsync($"/links/{Guid.NewGuid()}-non-existing/embed");

    // Assert
    var body = await r.Content.ReadAsStringAsync();
    body.Should().Contain("Not Found");
  }
}
