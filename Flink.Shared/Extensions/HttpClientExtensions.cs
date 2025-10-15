namespace Flink.Shared.Extensions;

public static class HttpClientExtensions {
  public static HttpClient AddBrowserImitationRequestHeaders(this HttpClient httpClient) {
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Linux x86_64; rv:143.0) Gecko/20100101 Firefox/143.0");
    httpClient.DefaultRequestHeaders.Accept.ParseAdd(
      "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
    httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br, zstd");
    httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Priority", "u=0, i");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Ch-Ua", "\"Not.A/Brand\";v=\"99\", \"Chromium\";v=\"136\"");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Ch-Ua-Mobile", "?0");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Ch-Ua-Platform", "\"Linux\"");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "document");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "navigate");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "none");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-User", "?1");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");

    return httpClient;
  }
}
