using System.Net;
using System.Text;
using System.Text.Json;
using Flink.IntegrationTests.TestScaffolding;
using Flink.ShortLinks.Models;
using FluentAssertions;

namespace Flink.IntegrationTests;

public class QrCodeGeneratorTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory> {
  [Fact]
  public async Task GetQrCodeRequest_RequestedShortLinkExists_ReturnsExpectedImage() {
    // Arrange
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = "ruffy",
      PreferredId = "blog",
      Uri = "https://flink.rtrace.io/"
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync("/links", payload);

    // Act
    var r = await httpClient.GetAsync("/links/blog/qr");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.OK);
    r.Content.Headers.ContentType?.MediaType.Should().Be("image/png");
  }

  [Fact]
  public async Task GetQrCodeRequest_NonExistingShortLink_Returns404() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync($"/links/{Guid.NewGuid()}-surely-not-existing/qr");

    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
    r.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
  }
}
