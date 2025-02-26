using System.Net.Http.Headers;
using Flink.Metrics.Ports.Models;
using Flink.Shared;
using Microsoft.Extensions.Options;

namespace Flink.Metrics.Ports.Middleware;

public class MetricsMiddleware(IOptions<MetricsConfiguration> metricsConfiguration, RequestDelegate next) {
  public async Task Invoke(HttpContext context) {
    if (!context.Request.Path.StartsWithSegments(metricsConfiguration.Value.MetricsEndpointPath)) {
      await next(context);
      return;
    }

    if (!metricsConfiguration.Value.EnableMetrics) {
      await ReturnNotFound(context);
      return;
    }

    if (!metricsConfiguration.Value.RequireBasicAuth) {
      await next(context);
      return;
    }

    if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader)) {
      await ReturnUnauthorized(context);
      return;
    }

    if (!AuthenticationHeaderValue.TryParse(authHeader, out var authValue)) {
      await ReturnUnauthorized(context);
      return;
    }

    var userGivenCredential = BasicAuth.GetCredential(authValue.ToString());
    if (userGivenCredential.IsFailure) {
      await ReturnUnauthorized(context);
      return;
    }

    var requiredCredential = new Credential {
      Username = metricsConfiguration.Value.BasicAuthUsername,
      Password = metricsConfiguration.Value.BasicAuthPassword
    };

    if (!requiredCredential.Equals(userGivenCredential.Value)) {
      await ReturnUnauthorized(context);
      return;
    }

    await next(context);
  }

  private static async Task ReturnUnauthorized(HttpContext context) {
    context.Response.Headers.WWWAuthenticate = "Basic realm=\"Metrics\"";
    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    await context.Response.WriteAsync("Unauthorized");
  }

  private static async Task ReturnNotFound(HttpContext context) {
    context.Response.StatusCode = StatusCodes.Status404NotFound;
    await context.Response.WriteAsync("Not Found");
  }
}
