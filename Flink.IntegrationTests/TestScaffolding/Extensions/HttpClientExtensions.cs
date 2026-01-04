using System.Text;
using System.Text.Json;
using Flink.ShortLinks.Models;

namespace Flink.IntegrationTests.TestScaffolding.Extensions;

public static class HttpClientExtensions {
  public static async Task<HttpResponseMessage> CreateShortLinkWithPreferredId(
    this HttpClient httpClient,
    string preferredId,
    string? password = null,
    DateTimeOffset? activeFrom = null,
    DateTimeOffset? activeUntil = null,
    JsonSerializerOptions? options = null,
    Encoding? encoding=null) {
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      PreferredId = preferredId,
      Uri = TestConstants.TestUri,
      ActiveUntil = activeUntil,
      ActiveFrom = activeFrom,
      Password = password
    }, options), encoding ?? Encoding.UTF8, "application/json");

    return await httpClient.PostAsync("/links", payload);
  }

  public static async Task<HttpResponseMessage> CreateShortLinkWithRandomId(
    this HttpClient httpClient,
    string? password = null,
    DateTimeOffset? activeFrom = null,
    DateTimeOffset? activeUntil = null,
    JsonSerializerOptions? options = null,
    Encoding? encoding = null) {
    var payload = new StringContent(JsonSerializer.Serialize(new ShortLinkRequest {
      Creator = Guid.NewGuid().ToString(),
      Uri = TestConstants.TestUri,
      ActiveUntil = activeUntil,
      ActiveFrom = activeFrom,
      Password = password,
      PreferredIdLength = 10
    }, options), encoding ?? Encoding.UTF8, "application/json");

    return await httpClient.PostAsync("/links", payload);
  }
}
