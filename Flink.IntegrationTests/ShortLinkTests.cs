using System.Net;
using System.Text;
using System.Text.Json;
using Flink.IntegrationTests.TestScaffolding;
using Flink.ShortLinks.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

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

  [Theory]
  [InlineData("")]
  [InlineData("/links")]
  public async Task GetShortLinkRequest_WhenRequestedShortLinkDoesNotExist_Returns404(string shortLinkPrefix) {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync($"{shortLinkPrefix}/{Guid.NewGuid()}-not-existing-{Guid.NewGuid()}");

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
      .PostAsync("/links", payload);

    r.StatusCode.Should().Be(HttpStatusCode.Created);
  }

  [Theory]
  [MemberData(nameof(SerializerSettings))]
  public async Task AddPreviouslyNonExistingShortLinkRequest_WithDifferentSerializationFormats_Returns201(JsonSerializerOptions opts) {
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = Guid.NewGuid().ToString(),
      Uri = "https://flink.rtrace.io/"
    }, opts), Encoding.UTF8, "application/json");

    var r = await webApplicationFactory
      .CreateClient()
      .PostAsync("/links", payload);

    r.StatusCode.Should().Be(HttpStatusCode.Created);
  }

  [Theory]
  [InlineData("")]
  [InlineData("/links")]
  public async Task GetExistingShortLinkRequest_WhenRequestedUriExists_Returns302(string shortLinkPrefix) {
    // Arrange
    var id = Guid.NewGuid().ToString();
    const string Uri = "https://flink.rtrace.io/";
    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = id,
      Uri = Uri
    }), Encoding.UTF8, "application/json");
    await httpClient.PostAsync("/links", payload);

    // Act
    var r = await httpClient.GetAsync($"{shortLinkPrefix}/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Redirect);
    r.Headers.Location.Should().Be(Uri);
  }

  [Fact]
  public async Task AddMultipleIdenticalExistingShortLinkRequest_WithIdenticalId_Returns400() {
    // Arrange
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = Guid.NewGuid().ToString(),
      Uri = "https://flink.rtrace.io/"
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync("/links", payload);

    // Act
    var r = await httpClient.PostAsync("/links", payload);

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory]
  [InlineData("/")]
  [InlineData("to")]
  [InlineData("metrics")]
  [InlineData("scalar")]
  [InlineData("swagger")]
  [InlineData("index")]
  [InlineData("index.html")]
  [InlineData("meta")]
  [InlineData("embed")]
  [InlineData("")]
  [InlineData("proxy")]
  [InlineData("info")]
  [InlineData("Info")]
  [InlineData("qr")]
  [InlineData("stats")]
  [InlineData("spam-protection")]
  [InlineData("pw")]
  [InlineData("history")]
  public async Task AddForbiddenShortLinkId_ToShortLinks_Returns400(string shortLinkId) {
    // Arrange
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = shortLinkId,
      Uri = "https://flink.rtrace.io/"
    }), Encoding.UTF8, "application/json");

    // Act
    var httpClient = webApplicationFactory.CreateClient();
    var r = await httpClient.PostAsync("/links", payload);

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task FlinkSetup_AttemptDeleteNotExisting_Returns200() {
    // Arrange
    var httpClient = webApplicationFactory.CreateClient();
    // Act
    var r = await httpClient.DeleteAsync($"/links/{Guid.NewGuid().ToString()}-delete");
    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task ExistingResourceDeleted_AttemptToAccessDeletedShortLink_Returns404() {
    // Arrange
    var id = $"{Guid.NewGuid()}-{nameof(ExistingResourceDeleted_AttemptToAccessDeletedShortLink_Returns404)}";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = id,
      Uri = "https://flink.rtrace.io/"
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.PostAsync("/links", payload);
    await httpClient.DeleteAsync($"/links/{id}");

    // Act
    var r = await httpClient.GetAsync($"/links/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task NewResourceCreated_NotYetActiveShortLink_Returns301() {
    // Arrange
    var id = $"{Guid.NewGuid()}-{nameof(NewResourceCreated_NotYetActiveShortLink_Returns301)}";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = id,
      Uri = "https://flink.rtrace.io/",
      ActiveFrom = DateTime.UtcNow.AddMinutes(1)
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    await httpClient.PostAsync("/links", payload);

    // Act
    var r = await httpClient.GetAsync($"/links/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Redirect);
  }

  [Fact]
  public async Task NewResourceCreated_AlreadyExpiredShortLink_Returns404() {
    // Arrange
    var id = $"{Guid.NewGuid()}-{nameof(NewResourceCreated_AlreadyExpiredShortLink_Returns404)}";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = id,
      Uri = "https://flink.rtrace.io/",
      ActiveUntil = DateTime.UtcNow.AddMinutes(-1)
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    await httpClient.PostAsync("/links", payload);

    // Act
    var r = await httpClient.GetAsync($"/links/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task NewResourceWithPasswordCreated_AccessedWithPassword_Returns301() {
    // Arrange
    const string Password = "123";
    var id = $"{Guid.NewGuid()}-{nameof(NewResourceWithPasswordCreated_AccessedWithPassword_Returns301)}";
    const string Uri = "https://flink.rtrace.io/";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = id,
      Uri = Uri,
      Password = Password
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    await httpClient.PostAsync("/links", payload);

    // Act
    var r = await httpClient.GetAsync($"/links/{id}?password={Password}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Redirect);
    r.Headers.Location.Should().Be(Uri);
  }

  [Fact]
  public async Task NewResourceWithPasswordCreated_AccessedWithoutPassword_Returns301() {
    // Arrange
    var id = $"{Guid.NewGuid()}-{nameof(NewResourceWithPasswordCreated_AccessedWithoutPassword_Returns301)}";
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = id,
      Uri = "https://flink.rtrace.io/",
      Password = "123"
    }), Encoding.UTF8, "application/json");

    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    await httpClient.PostAsync("/links", payload);

    // Act
    var r = await httpClient.GetAsync($"/links/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Redirect);
    r.Headers.Location.Should().Be($"/pw/{id}");
  }
}
