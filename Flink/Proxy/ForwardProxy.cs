using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Flink.Proxy;

public sealed class ForwardProxy(HttpClient httpClient) : IForwardProxy {
  public async Task<Result<FileContentResult>> Proxy(string url) {
    try {
      var response = await httpClient.GetAsync(url);
      var content = await response.Content.ReadAsByteArrayAsync();
      var contentType = response.Content.Headers.ContentType?.ToString() ?? "text/plain";
      return Result.Success(new FileContentResult(content, contentType));
    } catch (HttpRequestException exc) {
      return Result.Failure<FileContentResult>(exc.Message);
    }
  }
}
