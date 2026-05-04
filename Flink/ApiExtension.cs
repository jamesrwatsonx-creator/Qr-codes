using System.Net;
using Flink.Shared.ApiResponse;
using Microsoft.AspNetCore.Mvc;

namespace Flink;

public static class ApiExtension {
  public static IActionResult ToResult<T>(this ApiResponse<T> r) {
    if (r is { Location: not null, HttpStatusCode: (int)HttpStatusCode.Created }) {
      return new CreatedResult(r.Location, r);
    }

    return new ObjectResult(r) {
      StatusCode = r.HttpStatusCode
    };
  }

  public static string GetPublicBaseUrl(this HttpRequest request) {
    var forwardedHost = request.Headers["X-Forwarded-Host"]
      .FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();

    var forwardedProto = request.Headers["X-Forwarded-Proto"]
      .FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();

    var forwardedPort = request.Headers["X-Forwarded-Port"]
      .FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();

    var scheme = forwardedProto ?? request.Scheme;
    var host = forwardedHost ?? request.Host.Host;

    if (host.Contains(':')) return $"{scheme}://{host}";
    var port = forwardedPort ?? request.Host.Port?.ToString();

    if (string.IsNullOrEmpty(port)) return $"{scheme}://{host}";
    var isStandardPort = (scheme == "http" && port == "80") || (scheme == "https" && port == "443");

    if (!isStandardPort)
      host = $"{host}:{port}";

    return $"{scheme}://{host}";
  }
}
