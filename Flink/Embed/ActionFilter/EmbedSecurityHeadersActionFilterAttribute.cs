using Microsoft.AspNetCore.Mvc.Filters;

namespace Flink.Embed.ActionFilter;

public class EmbedSecurityHeadersActionFilterAttribute : ActionFilterAttribute {
  public override void OnResultExecuting(ResultExecutingContext context) {
    var headers = context.HttpContext.Response.Headers;

    headers.XContentTypeOptions = "nosniff";
    headers.ContentSecurityPolicy = "default-src 'self'; frame-ancestors *;";
    headers.XXSSProtection = "1; mode=block";

    headers["Cross-Origin-Embedder-Policy"] = "unsafe-none";
    headers["Cross-Origin-Opener-Policy"] = "unsafe-none";
    headers["Cross-Origin-Resource-Policy"] = "cross-origin";

    headers["Referrer-Policy"] = "no-referrer";
    headers["Permissions-Policy"] =
      "accelerometer=(), " +
      "ambient-light-sensor=(), " +
      "autoplay=(), " +
      "battery=(), " +
      "camera=(), " +
      "clipboard-write=(), " +
      "display-capture=(), " +
      "document-domain=(), " +
      "encrypted-media=(), " +
      "fullscreen=(), " +
      "geolocation=(), " +
      "gyroscope=(), " +
      "magnetometer=(), " +
      "microphone=(), " +
      "midi=(), " +
      "payment=(), " +
      "picture-in-picture=(), " +
      "publickey-credentials-get=(), " +
      "sync-xhr=(), " +
      "usb=(), " +
      "wake-lock=(), " +
      "xr-spatial-tracking=()";

    base.OnResultExecuting(context);
  }
}
