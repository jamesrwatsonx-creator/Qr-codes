using Microsoft.AspNetCore.Http;
using FluentAssertions;

namespace Flink.UnitTests;

public class ApiExtensionTests {
  private static HttpRequest CreateRequest(
    string scheme = "http",
    string host = "example.com",
    int? port = null) {
    var context = new DefaultHttpContext();
    var request = context.Request;

    request.Scheme = scheme;
    request.Host = port.HasValue
      ? new HostString(host, port.Value)
      : new HostString(host);

    return request;
  }

  [Fact]
  public void Given_NoForwardedHeaders_When_GetPublicBaseUrl_Then_UsesRequestSchemeAndHost() {
    // Arrange
    var request = CreateRequest("https", "mydomain.com");

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("https://mydomain.com");
  }

  [Fact]
  public void Given_ForwardedHostHeader_When_GetPublicBaseUrl_Then_UsesForwardedHost() {
    // Arrange
    var request = CreateRequest();
    request.Headers["X-Forwarded-Host"] = "forwarded.com";

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("http://forwarded.com");
  }

  [Fact]
  public void Given_ForwardedProtoHeader_When_GetPublicBaseUrl_Then_UsesForwardedProto() {
    // Arrange
    var request = CreateRequest("http", "example.com");
    request.Headers["X-Forwarded-Proto"] = "https";

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("https://example.com");
  }

  [Fact]
  public void Given_ForwardedPortHeader_When_NonStandardPort_Then_AppendsPort() {
    // Arrange
    var request = CreateRequest("http", "example.com");
    request.Headers["X-Forwarded-Port"] = "8080";

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("http://example.com:8080");
  }

  [Fact]
  public void Given_ForwardedPortHeader_When_StandardHttpPort_Then_DoesNotAppendPort() {
    // Arrange
    var request = CreateRequest("http", "example.com");
    request.Headers["X-Forwarded-Port"] = "80";

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("http://example.com");
  }

  [Fact]
  public void Given_ForwardedPortHeader_When_StandardHttpsPort_Then_DoesNotAppendPort() {
    // Arrange
    var request = CreateRequest("https", "example.com");
    request.Headers["X-Forwarded-Port"] = "443";

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("https://example.com");
  }

  [Fact]
  public void Given_HostAlreadyContainsPort_When_GetPublicBaseUrl_Then_DoesNotAppendPortAgain() {
    // Arrange
    var request = CreateRequest("http", "example.com:5000");

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("http://example.com:5000");
  }

  [Fact]
  public void Given_MultipleForwardedHostValues_When_GetPublicBaseUrl_Then_UsesFirstValue() {
    // Arrange
    var request = CreateRequest();
    request.Headers["X-Forwarded-Host"] = "first.com, second.com";

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("http://first.com");
  }

  [Fact]
  public void Given_NoForwardedPortButRequestHasPort_When_NonStandardPort_Then_AppendsPort() {
    // Arrange
    var request = CreateRequest("http", "example.com", 5000);

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("http://example.com:5000");
  }

  [Fact]
  public void Given_NoForwardedPortAndStandardHttpsPort_When_GetPublicBaseUrl_Then_DoesNotAppendPort() {
    // Arrange
    var request = CreateRequest("https", "example.com", 443);

    // Act
    var result = request.GetPublicBaseUrl();

    // Assert
    result.Should().Be("https://example.com");
  }
}
