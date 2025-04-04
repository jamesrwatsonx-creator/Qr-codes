using Microsoft.AspNetCore.Mvc.Filters;

namespace Flink.WebUI.ActionFilter;

public class WebUiSecurityHeadersActionFilterAttribute : ActionFilterAttribute {
  public override void OnResultExecuting(ResultExecutingContext context) {
    var headers = context.HttpContext.Response.Headers;

    headers.XContentTypeOptions = "nosniff";
    headers.XFrameOptions = "DENY";
    headers.ContentSecurityPolicy = "default-src 'self';";
    headers.XXSSProtection = "1; mode=block";
    
    headers["Cross-Origin-Embedder-Policy"] = "same-origin";
    headers["Cross-Origin-Opener-Policy"] = "same-origin";
    headers["Cross-Origin-Resource-Policy"] = "same-origin";
    
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
