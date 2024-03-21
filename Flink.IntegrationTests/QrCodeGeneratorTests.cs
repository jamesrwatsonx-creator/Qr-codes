using System.Net;
using System.Text;
using System.Text.Json;
using Flink.ShortLinks.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Flink.IntegrationTests;

public class QrCodeGeneratorTests(WebApplicationFactory<Program> webApplicationFactory) : IClassFixture<WebApplicationFactory<Program>> {
  [Fact]
  public async Task GetQrCodeRequest_RequestedShortLinkExists_ReturnsExpectedImage() {
    // Arrange
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = "ruffy",
      PreferredId = "blog",
      Uri = "https://flink.rtrace.io/"
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync($"/to", payload);

    // Act
    var r = await httpClient.GetAsync($"/qr/blog");
    
    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.OK);
    r.Content.Headers.ContentType?.MediaType.Should().Be("image/png");
  }
  
  [Fact]
  public async Task GetQrCodeRequest_NonExistingShortLink_Returns404() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync($"/qr/{Guid.NewGuid()}-surely-not-existing");
    
    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
    r.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
  }
}
