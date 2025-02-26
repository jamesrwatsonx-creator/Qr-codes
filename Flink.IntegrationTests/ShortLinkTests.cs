using System.Net;
using System.Text;
using System.Text.Json;
using Flink.IntegrationTests.TestScaffolding;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Flink.ShortLinks.Models;

namespace Flink.IntegrationTests;

public class ShortLinkTests(FlinkWebApplicationFactory webApplicationFactory) : IClassFixture<FlinkWebApplicationFactory> {
  public static readonly IEnumerable<object[]> SerializerSettings = new List<object[]> {
    new object[] { new JsonSerializerOptions() },
    new object[] { new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase } },
    new object[] { new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower } },
    new object[] { new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.KebabCaseUpper } },
    new object[] { new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower } },
    new object[] { new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper } }
  };

  [Fact]
  public async Task GetShortLinkRequest_WhenRequestedShortLinkDoesNotExist_Returns404() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync($"/to/{Guid.NewGuid()}-not-existing-{Guid.NewGuid()}");

    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task AddShortLinkRequest_WhenProvidedPreferredUrlDoesNotYetExist_Returns201() {
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = Guid.NewGuid().ToString(),
      Uri = "https://flink.rtrace.io/"
    }), Encoding.UTF8, "application/json");

    var r = await webApplicationFactory
      .CreateClient()
      .PostAsync($"/to", payload);

    r.StatusCode.Should().Be(HttpStatusCode.Created);
  }

  [Theory, MemberData(nameof(SerializerSettings))]
  public async Task AddPreviouslyNonExistingShortLinkRequest_WithDifferentSerializationFormats_Returns201(JsonSerializerOptions opts) {
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = Guid.NewGuid().ToString(),
      Uri = "https://flink.rtrace.io/"
    }, opts), Encoding.UTF8, "application/json");

    var r = await webApplicationFactory
      .CreateClient()
      .PostAsync($"/to", payload);

    r.StatusCode.Should().Be(HttpStatusCode.Created);
  }

  [Fact]
  public async Task GetExistingShortLinkRequest_WhenRequestedUriExists_Returns302() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    const string Uri = "https://flink.rtrace.io/";
    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = id,
      Uri = Uri,
    }), Encoding.UTF8, "application/json");
    await httpClient.PostAsync($"/to", payload);

    // Act
    var r = await httpClient.GetAsync($"/to/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Redirect);
    r.Headers.Location.Should().Be(Uri);
  }

  [Fact]
  public async Task AddMultipleIdenticalExistingShortLinkRequest_WithIdenticalId_Returns400() {
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = Guid.NewGuid().ToString(),
      Uri = "https://flink.rtrace.io/"
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync($"/to", payload);
    var r = await httpClient.PostAsync($"/to", payload);

    r.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}
