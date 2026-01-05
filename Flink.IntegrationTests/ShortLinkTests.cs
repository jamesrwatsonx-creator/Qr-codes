using System.Net;
using System.Text;
using System.Text.Json;
using Flink.Domain;
using Flink.IntegrationTests.TestScaffolding;
using Flink.IntegrationTests.TestScaffolding.Extensions;
using Flink.Shared.ApiResponse;
using Flink.ShortLinks;
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

  public static readonly IEnumerable<object[]> SupportedEncodings = new List<object[]> {
    new object[] { Encoding.UTF8 },
    new object[] { Encoding.Unicode }
  };

  public static readonly IEnumerable<object[]> ReservedIds = SpecialShortLinkIds.ReservedShortLinkIds.Select(x => new object[] { x });

  [Fact]
  public async Task AddShortLinkRequest_PreviouslyNotExistingWithPreferredId_Returns201Created() {
    // Act
    var r = await webApplicationFactory
      .CreateClient()
      .CreateShortLinkWithPreferredId(Guid.NewGuid().ToString());

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Created);
  }
  
  
  [Fact]
  public async Task AddShortLinkRequest_PreviouslyNotExistingWithRandomId_Returns201Created() {
    // Act
    var r = await webApplicationFactory
      .CreateClient()
      .CreateShortLinkWithRandomId();

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Created);
  }

  [Theory]
  [MemberData(nameof(SerializerSettings))]
  public async Task AddShortLinkRequest_PreviouslyNotExistingWithDifferentSerializationFormats_Returns201Created(JsonSerializerOptions opts) {
    // Act
    var r = await webApplicationFactory
      .CreateClient()
      .CreateShortLinkWithPreferredId(Guid.NewGuid().ToString(), options: opts);

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Created);
  }

  [Theory]
  [MemberData(nameof(SupportedEncodings))]
  public async Task AddShortLinkRequest_PreviouslyNotExistingWithDifferentRequestEncodings_Returns201Created(Encoding enc) {
    // Act
    var r = await webApplicationFactory.CreateClient()
      .CreateShortLinkWithPreferredId(Guid.NewGuid().ToString(), encoding: enc);

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Created);
  }
  
  
  [Fact]
  public async Task AddShortLinkRequest_TwiceWithIdenticalId_Returns400BadRequest() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    await httpClient.CreateShortLinkWithPreferredId(id);
    
    // Act
    var r = await httpClient.CreateShortLinkWithPreferredId(id);

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  
  [Theory]
  [InlineData("")]
  [InlineData("/links")]
  public async Task GetShortLinkRequest_WhenRequestedShortLinkDoesNotExist_Returns404(string shortLinkPrefix) {
    // Act
    var r = await webApplicationFactory.CreateClient().GetAsync($"{shortLinkPrefix}/{Guid.NewGuid()}-nx");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
  
  
  [Theory]
  [InlineData("")]
  [InlineData("/links")]
  public async Task GetShortLink_ExistingShortLink_Returns302Redirect(string shortLinkPrefix) {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    await httpClient.CreateShortLinkWithPreferredId(id);

    // Act
    var r = await httpClient.GetAsync($"{shortLinkPrefix}/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Redirect);
    r.Headers.Location.Should().Be(TestConstants.TestUri);
  }

  [Theory]
  [MemberData(nameof(ReservedIds))]
  public async Task AddShortLinkRequest_WithForbiddenAndReservedShortLinkId_Returns451Unavailable(string reservedShortLinkId) {
    // Arrange
    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    // Act
    var r = await httpClient.CreateShortLinkWithPreferredId(reservedShortLinkId);

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.UnavailableForLegalReasons);
  }

  [Fact]
  public async Task DeleteShortLinkRequest_DeleteNonExistingShortLink_Returns200Ok() {
    // Arrange
    var httpClient = webApplicationFactory.CreateClient();
    
    // Act
    var r = await httpClient.DeleteAsync($"/links/{Guid.NewGuid().ToString()}-delete");
    
    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Theory]
  [InlineData("")]
  [InlineData("/links")]
  public async Task DeleteShortLinkRequest_GetShortLinkAfterDelete_Returns404NotFound(string prefix) {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.CreateShortLinkWithPreferredId(id);
    await httpClient.DeleteAsync($"/links/{id}");

    // Act
    var r = await httpClient.GetAsync($"{prefix}{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task GetShortLinkRequest_GetOnNotYetActiveShortLink_Returns302Redirect() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions {AllowAutoRedirect = false});
    await httpClient.CreateShortLinkWithPreferredId(id, activeUntil: DateTimeOffset.UtcNow.AddMinutes(1));

    // Act
    var r = await httpClient.GetAsync($"/links/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Redirect);
  }

  [Fact]
  public async Task GetShortLinkRequest_GetOnAlreadyExpiredShortLink_Returns410Gone() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.CreateShortLinkWithPreferredId(id, activeUntil: DateTimeOffset.UtcNow.AddMinutes(-1));

    // Act
    var r = await httpClient.GetAsync($"/links/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Gone);
  }

  [Fact]
  public async Task GetShortLinkRequest_AccessedWithCorrectPassword_Returns302Redirect() {
    // Arrange
    const string Password = "123";
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions {AllowAutoRedirect = false});
    await httpClient.CreateShortLinkWithPreferredId(id, password: Password);

    // Act
    var r = await httpClient.GetAsync($"/links/{id}?password={Password}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Redirect);
    r.Headers.Location.Should().Be(TestConstants.TestUri);
  }

  [Fact]
  public async Task GetShortLinkRequest_AccessedWithoutPassword_Returns401Unauthorized() {
    // Arrange
    const string Password = "123";
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.CreateShortLinkWithPreferredId(id, password: Password);

    // Act
    var r = await httpClient.GetAsync($"/links/{id}");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }
  
  [Fact]
  public async Task GetShortLinkRequest_AccessedWithIncorrectPassword_Returns401Unauthorized() {
    // Arrange
    const string Password = "123";
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient();
    await httpClient.CreateShortLinkWithPreferredId(id, password: Password);

    // Act
    var r = await httpClient.GetAsync($"/links/{id}?password=WrongPassword");

    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }
  
  [Fact]
  public async Task AddShortLinkRequest_WithPassword_StripsPasswordFromResponsePayload() {
    // Arrange
    const string Password = "123";
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient();
    
    
    // Act
    var r = await httpClient.CreateShortLinkWithPreferredId(id, password: Password);
    var bodyText = await r.Content.ReadAsStringAsync();
    var shortenedLink = JsonSerializer.Deserialize<ApiResponse<ShortenedLink>>(bodyText, JsonSerializerOptions.Web);
    
    // Assert
    shortenedLink?.Data?.PasswordHash.Should().BeNull();
  }
  
  [Fact]
  public async Task AddShortLinkRequest_NotYetActive_StripsUriFromPayload() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient();
    
    
    // Act
    var r = await httpClient.CreateShortLinkWithPreferredId(id, activeFrom: DateTimeOffset.UtcNow.AddMinutes(1));
    var bodyText = await r.Content.ReadAsStringAsync();
    var shortenedLink = JsonSerializer.Deserialize<ApiResponse<ShortenedLink>>(bodyText, JsonSerializerOptions.Web);
    
    // Assert
    shortenedLink?.Data?.Uri.Should().NotBe(TestConstants.TestUri);
  }
  
  [Fact]
  public async Task AddShortLinkRequest_AlreadyExpired_StripsUriFromPayload() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient();
    
    
    // Act
    var r = await httpClient.CreateShortLinkWithPreferredId(id, activeUntil: DateTimeOffset.UtcNow.AddMinutes(-1));
    var bodyText = await r.Content.ReadAsStringAsync();
    var shortenedLink = JsonSerializer.Deserialize<ApiResponse<ShortenedLink>>(bodyText, JsonSerializerOptions.Web);
    
    // Assert
    shortenedLink?.Data?.Uri.Should().NotBe(TestConstants.TestUri);
  }
  
  [Fact]
  public async Task GetShortLinkRequest_BetweenActiveFromAndActiveUntil_Returns302Redirect() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var httpClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions{AllowAutoRedirect = false});
    await httpClient.CreateShortLinkWithPreferredId(id, activeFrom: DateTimeOffset.UtcNow.AddMinutes(-1), activeUntil: DateTimeOffset.UtcNow.AddMinutes(1));
    
    // Act
    var r = await httpClient.GetAsync($"/links/{id}");
    
    // Assert
    r.StatusCode.Should().Be(HttpStatusCode.Redirect);
    r.Headers.Location?.ToString().Should().Be(TestConstants.TestUri);
  }
}
