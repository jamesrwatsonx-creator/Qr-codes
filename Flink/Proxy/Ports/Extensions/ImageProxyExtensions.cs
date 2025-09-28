namespace Flink.Proxy.Ports.Extensions;

public static class ImageProxyExtensions {
  public static string GetThumbnailUrl(this HttpRequest request, string shortLinkId) =>
    $"{request.Scheme}://{request.Headers.Host}/links/{shortLinkId}/thumbnail";
}
