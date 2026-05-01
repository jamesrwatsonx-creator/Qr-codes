using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData.Extensions;

namespace Flink.MetaData;

public class MetaDataScraper(HttpClient httpClient) : IMetaDataScraper {
  public async Task<Result<ShortLinkMetaData>> ScrapeAsync(ShortenedLink shortenedLink) {
    try {
      var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5000));
      var r = await httpClient.GetAsync(shortenedLink.Uri, cts.Token);
      
      var contentType = r.Content.Headers.ContentType?.MediaType?.ToLower() ?? "";

      IDictionary<string, string> metaDict;

      if (contentType.StartsWith("text/html")) {
        var body = await r.Content.ReadAsStringAsync(cts.Token);
        metaDict = HtmlMetaDataExtractor.ExtractMetadata(body);
      }
      else if (contentType.StartsWith("image/")) {
        metaDict = ImageMetaDataExtractor.Extract(new Uri(shortenedLink.Uri));
      }
      else if (contentType.StartsWith("application/pdf") || shortenedLink.Uri.ToLowerInvariant().EndsWith(".pdf") ) {
        var body = await r.Content.ReadAsByteArrayAsync(cts.Token);
        metaDict = PdfMetaDataExtractor.ExtractMetadata(body);
      }
      else {
        metaDict = new Dictionary<string, string>();
      }
 
      return Result.Success(new ShortLinkMetaData {
        Id = shortenedLink.Id,
        Author = metaDict.GetOrDefault(["author", "article:author"], null),
        Description = metaDict.GetOrDefault(["description", "og:description", "twitter:description"], null),
        Image = metaDict.GetOrDefault(["og:image", "twitter:image", "logo"], null),
        Title = metaDict.GetOrDefault(["title", "og:description", "twitter:description"], null),
        HttpStatusCode = (int)r.StatusCode,
        DocumentSize = r.Content.Headers.ContentLength > int.MaxValue ? int.MaxValue : (int)(r.Content.Headers.ContentLength ?? 0),
        LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
      });
    } catch (Exception e) {
      return Result.Failure<ShortLinkMetaData>(e.Message);
    }
  }
}
