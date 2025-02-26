using System.Net;
using System.Text;
using System.Text.Json;
using Flink.Domain;
using Flink.IntegrationTests.TestScaffolding;
using Flink.ShortLinks.Models;
using FluentAssertions;

namespace Flink.IntegrationTests;

public class MetaDataTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory> {
  private readonly JsonSerializerOptions _serializerOptions = new() {
    PropertyNameCaseInsensitive = true
  };

  [Fact]
  public async Task FlinkIsRunningWithoutAnyScrapedMetaData_MetaDataForNonExistingUrlIsRequested_Returns404() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync($"/meta/{Guid.NewGuid()}-{Guid.NewGuid()}-Non-Existing");

    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task FlinkIsRunning_SelfLinkAddedAsShortLink_ReturnsOkMetaData() {
    // Arrange
    var shortLinkId = $"noice{Guid.NewGuid()}";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = "ruffy",
      PreferredId = shortLinkId,
      Uri = "https://blog.rtrace.io/",
      PreferredIdLength = 14
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync($"/to", payload);

    // Act
    await Task.Delay(TimeSpan.FromSeconds(2));
    var metaRequest = await httpClient.GetAsync($"/meta/{shortLinkId}");

    // Assert
    metaRequest.StatusCode.Should().Be(HttpStatusCode.OK);
    var responseContent = await metaRequest.Content.ReadAsStringAsync();
    var deserializedResponse = JsonSerializer.Deserialize<ShortLinkMetaData>(responseContent, _serializerOptions);
    deserializedResponse?.Title.Should().NotBeNull();
  }
}
