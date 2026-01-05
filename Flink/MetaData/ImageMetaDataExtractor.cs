namespace Flink.MetaData;

public static class ImageMetaDataExtractor {
  public static IDictionary<string, string> Extract(Uri uri) {
    var fileName = Path.GetFileName(uri.LocalPath);

    return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
      ["title"] = string.IsNullOrWhiteSpace(fileName) ? uri.Host : fileName,
      ["og:image"] = uri.ToString()
    };
  }
}
