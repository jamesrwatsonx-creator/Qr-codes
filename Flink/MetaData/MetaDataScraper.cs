using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData.Extensions;

namespace Flink.MetaData;

public class  MetaDataScraper(HttpClient httpClient) : IMetaDataScraper {
  public async Task<Result<ShortLinkMetaData>> ScrapeAsync(ShortenedLink shortenedLink) {
    try {
      var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
      var httpRequest = await httpClient.GetAsync(shortenedLink.Uri, cts.Token);
      var body = await httpRequest.Content.ReadAsStringAsync(cts.Token);
      var metaDict = HtmlMetaDataExtractor.ExtractMetadata(body);

      return Result.Success(new ShortLinkMetaData {
        ShortLinkId = shortenedLink.Id,
        Author = metaDict.GetOrDefault(["author", "article:author"], null),
        Description = metaDict.GetOrDefault(["description", "og:description", "twitter:description"], null),
        Image = metaDict.GetOrDefault(["og:image", "twitter:image"], null),
        Title = metaDict.GetOrDefault(["title", "og:description", "twitter:description"], null),
        HttpStatusCode = (int)httpRequest.StatusCode,
        DocumentSize = body.Length,
        LastUpdate = DateTime.UtcNow
      });
    } catch (Exception e) {
      return Result.Failure<ShortLinkMetaData>(e.Message);
    }
  }
}
