using HtmlAgilityPack;

namespace Flink.MetaData;

public static class HtmlMetaDataExtractor {
  private static readonly string[] MetaDataKeysOfInterest = [
    "description", "author",
    "article:author",
    "og:title", "og:description", "og:image",
    "twitter:title", "twitter:description", "twitter:image"
  ];
  
  
  public static IDictionary<string, string> ExtractMetadata(string htmlBody) {
    try {
      var doc = new HtmlDocument();
      doc.LoadHtml(htmlBody);
      var metaDict = doc.DocumentNode
        .SelectNodes("//head//meta")
        .Select(ToKeyValuePair)
        .Where(kvp => kvp != null)
        .Select(kvp => kvp!.Value)
        .Where(kvp => MetaDataKeysOfInterest.Contains(kvp.Key))
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

      metaDict["title"] = doc.DocumentNode.SelectSingleNode("//head//title").InnerText;
      return metaDict;
    } catch {
      return new Dictionary<string, string>();
    }
  }

  private static KeyValuePair<string, string>? ToKeyValuePair(HtmlNode n) {
    try {
      if (n.Attributes.Contains("name"))
        return new KeyValuePair<string, string>(n.Attributes["name"].Value, n.Attributes["content"].Value);
      if (n.Attributes.Contains("property"))
        return new KeyValuePair<string, string>(n.Attributes["property"].Value, n.Attributes["content"].Value);
    } catch { /* ignored */ }

    return null;

  }
}
