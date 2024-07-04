using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Flink.Proxy;

public sealed class ForwardProxy(HttpClient httpClient) : IForwardProxy {
  public async Task<Result<FileContentResult>> Proxy(string url) {
    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    try {
      var response = await httpClient.GetAsync(url, cts.Token);
      var content = await response.Content.ReadAsByteArrayAsync(cts.Token);
      var contentType = response.Content.Headers.ContentType?.ToString() ?? "text/plain";
      return Result.Success(new FileContentResult(content, contentType));
    } catch (HttpRequestException exc) {
      return Result.Failure<FileContentResult>(exc.Message);
    }
  }
}
