using System.Net;
using System.Text;
using System.Text.Json;
using Flink.IntegrationTests.TestScaffolding;
using Flink.ShortLinks.Models;

namespace Flink.IntegrationTests;

public class MetaDataTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory> {
  private readonly JsonSerializerOptions _serializerOptions = new() {
    PropertyNameCaseInsensitive = true
  };

  [Fact]
  public async Task FlinkIsRunningWithoutAnyScrapedMetaData_MetaDataForNonExistingUrlIsRequested_Returns404() {
    // Act
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync($"/links/{Guid.NewGuid()}-{Guid.NewGuid()}-Non-Existing/meta");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task FlinkIsRunning_SelfLinkAddedAsShortLink_ReturnsOkMetaData() {
    // Arrange
    var shortLinkId = $"noice{Guid.NewGuid()}";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/"
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync("/links", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(4));
    var metaRequest = await httpClient.GetAsync($"/links/{shortLinkId}/meta");

    // Assert
    metaRequest.StatusCode.Should().Be(HttpStatusCode.OK);
    var responseContent = await metaRequest.Content.ReadAsStringAsync();
    var deserializedResponse = JsonSerializer.Deserialize<ShortLinkMetaData>(responseContent, _serializerOptions);
    deserializedResponse?.Title.Should().NotBeNull();
  }

  [Fact]
  public async Task FlinkIsRunningShortLinkWithPasswordAdded_RequestedWithoutPassword_Returns401() {
    // Arrange
    var shortLinkId = $"noice{Guid.NewGuid()}";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/",
      Password = Guid.NewGuid().ToString()
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync("/links", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(4));
    var metaRequest = await httpClient.GetAsync($"/links/{shortLinkId}/meta");

    // Assert
    metaRequest.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task FlinkIsRunningShortLinkWithPasswordAdded_RequestedWithPassword_Returns200() {
    // Arrange
    var shortLinkId = $"noice{Guid.NewGuid()}";
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
    var metaRequest = await httpClient.GetAsync($"/links/{shortLinkId}/meta?password={password}");

    // Assert
    metaRequest.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task Embed_ForNonExistingShortLink_Returns404Embed() {
    // Act
    var response = await webApplicationFactory
      .CreateClient()
      .GetAsync($"/links/{Guid.NewGuid()}-{Guid.NewGuid()}/embed");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK); // always HTML
    var content = await response.Content.ReadAsStringAsync();
    content.Should().Contain("404");
  }

  [Fact]
  public async Task Embed_ForValidShortLink_Returns200Html() {
    // Arrange
    var shortLinkId = $"embed{Guid.NewGuid()}";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/"
    }), Encoding.UTF8, "application/json");

    var client = webApplicationFactory.CreateClient();
    await client.PostAsync("/links", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(4));
    var response = await client.GetAsync($"/links/{shortLinkId}/embed");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var html = await response.Content.ReadAsStringAsync();
    html.Should().Contain(shortLinkId);
  }

  [Fact]
  public async Task Embed_ForPasswordProtectedShortLink_WithoutPassword_Returns401Embed() {
    // Arrange
    var shortLinkId = $"embed{Guid.NewGuid()}";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/",
      Password = Guid.NewGuid().ToString()
    }), Encoding.UTF8, "application/json");

    var client = webApplicationFactory.CreateClient();
    await client.PostAsync("/links", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(4));
    var response = await client.GetAsync($"/links/{shortLinkId}/embed");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var html = await response.Content.ReadAsStringAsync();
    html.Should().Contain("Unauthorized");
  }

  [Fact]
  public async Task Embed_ForPasswordProtectedShortLink_WithPassword_Returns200() {
    // Arrange
    var shortLinkId = $"embed{Guid.NewGuid()}";
    var password = Guid.NewGuid().ToString();

    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/",
      Password = password
    }), Encoding.UTF8, "application/json");

    var client = webApplicationFactory.CreateClient();
    await client.PostAsync("/links", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(4));
    var response = await client.GetAsync($"/links/{shortLinkId}/embed?password={password}");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var html = await response.Content.ReadAsStringAsync();
    html.Should().Contain(shortLinkId);
  }

  [Fact]
  public async Task Embed_ForExpiredShortLink_Returns410Embed() {
    // Arrange
    var shortLinkId = $"embed{Guid.NewGuid()}";

    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/",
      ActiveUntil = DateTimeOffset.UtcNow.AddSeconds(-10)
    }), Encoding.UTF8, "application/json");

    var client = webApplicationFactory.CreateClient();
    await client.PostAsync("/links", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(2));
    var response = await client.GetAsync($"/links/{shortLinkId}/embed");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var html = await response.Content.ReadAsStringAsync();
    html.Should().Contain("Gone");
  }

  [Fact]
  public async Task Embed_ForNotYetActiveShortLink_Returns503Embed() {
    // Arrange
    var shortLinkId = $"embed{Guid.NewGuid()}";

    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/",
      ActiveFrom = DateTimeOffset.UtcNow.AddMinutes(5)
    }), Encoding.UTF8, "application/json");

    var client = webApplicationFactory.CreateClient();
    await client.PostAsync("/links", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(2));
    var response = await client.GetAsync($"/links/{shortLinkId}/embed");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var html = await response.Content.ReadAsStringAsync();
    html.Should().Contain("not yet active");
  }
}
