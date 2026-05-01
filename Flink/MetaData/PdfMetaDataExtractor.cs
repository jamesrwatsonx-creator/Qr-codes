using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Filters;
using UglyToad.PdfPig.Tokens;

namespace Flink.MetaData;

public static class PdfMetaDataExtractor {
  private static readonly HashSet<string> MetaDataKeysOfInterest =
    new(StringComparer.OrdinalIgnoreCase) {
      "Title",
      "Author",
      "Subject",
      "Keywords",
      "Creator",
      "Producer",
      "CreationDate",
      "ModDate",
      "Description"
    };

  public static IDictionary<string, string> ExtractMetadata(byte[] pdfBytes) {
    try {
      using var stream = new MemoryStream(pdfBytes);
      using var document = PdfDocument.Open(stream);

      var metaDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      ExtractInfoDictionary(document, metaDict);
      ExtractXmpMetadata(document, metaDict);

      var firstPageText = GetTextFromFirstPage(document);
      
      if (!metaDict.TryGetValue("title", out var value) || string.IsNullOrWhiteSpace(value)) {
        var title = GetTitleFromLargestTextBlock(document);

        if (string.IsNullOrWhiteSpace(title))
          title = GetBestTitleFallback(firstPageText);

        if (!string.IsNullOrWhiteSpace(title))
          metaDict["title"] = title;
      }

      if (!metaDict.ContainsKey("description")) {
        var description = GetDescription(firstPageText, 500);
        if (!string.IsNullOrWhiteSpace(description))
          metaDict["description"] = description;
      }

      metaDict.TryAdd("title", "Unknown");
      metaDict.TryAdd("logo", "/static/img/files/pdf.png");
      return metaDict;
    } catch {
      return new Dictionary<string, string>();
    }
  }
  
  private static string GetTitleFromLargestTextBlock(PdfDocument document) {
    try {
      var page = document.GetPage(1);
      var letters = page.Letters;

      if (letters.Count == 0)
        return string.Empty;

      // Group letters into lines by Y position
      var lines = letters
        .GroupBy(l => Math.Round(l.BoundingBox.Bottom, 1))
        .Select(g => new {
          Letters = g.OrderBy(l => l.BoundingBox.Left).ToList()
        })
        .Select(g => new {
          Text = string.Concat(g.Letters.Select(l => l.Value)),
          AvgHeight = g.Letters.Average(l => l.BoundingBox.Height),
          WordCount = g.Letters.Count(l => l.Value == " ") + 1
        })
        .Where(l => !string.IsNullOrWhiteSpace(l.Text))
        .Select(l => new {
          Text = Normalize(l.Text),
          l.AvgHeight,
          l.WordCount,
          Score = ScoreTextBlock(l.Text, l.AvgHeight, l.WordCount)
        })
        .OrderByDescending(l => l.Score)
        .ThenByDescending(l => l.AvgHeight)
        .Take(5)
        .ToList();

      return lines.FirstOrDefault()?.Text ?? string.Empty;
    } catch {
      return string.Empty;
    }
  }

  private static double ScoreTextBlock(string text, double fontSize, int wordCount) {
    double score = 0;

    var length = text.Length;

    // Font size = strongest signal
    score += fontSize * 2;

    // Ideal title length
    if (length >= 10 && length <= 120)
      score += 3;

    // Word count sweet spot
    if (wordCount >= 2 && wordCount <= 12)
      score += 2;

    // Penalize all caps (but not too harsh)
    if (text.ToUpperInvariant() == text)
      score -= 1;

    // Penalize noisy lines
    if (Regex.IsMatch(text, @"[^a-zA-Z0-9\s\-\,:]"))
      score -= 1;

    // Penalize long paragraphs
    if (length > 150)
      score -= 3;

    return score;
  }

  private static string Normalize(string text) {
    return Regex.Replace(text, @"\s+", " ").Trim();
  }
  
  private static string GetBestTitleFallback(string text) {
    if (string.IsNullOrWhiteSpace(text))
      return string.Empty;

    text = Normalize(text);

    var lines = text
      .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
      .Select(l => l.Trim())
      .Where(l => l.Length > 5)
      .Take(20)
      .ToList();

    string? bestLine = null;
    double bestScore = 0;

    foreach (var line in lines) {
      var score = ScoreFallbackLine(line);

      if (score > bestScore) {
        bestScore = score;
        bestLine = line;
      }
    }

    return bestLine ?? GetFirstSentenceFallback(text);
  }

  private static double ScoreFallbackLine(string line) {
    double score = 0;

    var length = line.Length;
    var wordCount = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    if (length is >= 20 and <= 120)
      score += 2;

    if (wordCount is >= 3 and <= 12)
      score += 2;

    if (line.ToUpperInvariant() == line)
      score -= 1.5;

    if (length > 150)
      score -= 2;

    return score;
  }

  private static string GetFirstSentenceFallback(string text) {
    var sentences = Regex.Split(text, @"(?<=[.!?])\s+")
      .Select(s => s.Trim())
      .Where(s => s.Length > 15)
      .ToList();

    return sentences.FirstOrDefault() ?? string.Empty;
  }

  private static string GetTextFromFirstPage(PdfDocument document) {
    try {
      return document.GetPage(1).Text ?? string.Empty;
    } catch {
      return string.Empty;
    }
  }

  private static string GetDescription(string text, int maxLength) {
    if (string.IsNullOrWhiteSpace(text))
      return string.Empty;

    text = Normalize(text);

    return text.Length <= maxLength
      ? text
      : text[..maxLength];
  }

  private static void ExtractInfoDictionary(PdfDocument document, IDictionary<string, string> dict) {
    try {
      var info = document.Information;

      AddIfNotEmpty(dict, "title", info.Title);
      AddIfNotEmpty(dict, "author", info.Author);
      AddIfNotEmpty(dict, "subject", info.Subject);
      AddIfNotEmpty(dict, "keywords", info.Keywords);
      AddIfNotEmpty(dict, "creator", info.Creator);
      AddIfNotEmpty(dict, "producer", info.Producer);
    } catch { /* ignored */ }
  }

  private static void ExtractXmpMetadata(PdfDocument document, IDictionary<string, string> dict) {
    try {
      var catalog = document.Structure?.Catalog;
      if (catalog == null) return;

      var dictToken = catalog.CatalogDictionary;

      if (!dictToken.TryGet(NameToken.Metadata, out var metadataToken))
        return;

      if (metadataToken is not StreamToken streamToken)
        return;

      var decoded = streamToken.Decode(DefaultFilterProvider.Instance);
      var xml = Encoding.UTF8.GetString(decoded.ToArray());
      var xdoc = XDocument.Parse(xml);

      XNamespace dc = "http://purl.org/dc/elements/1.1/";
      XNamespace pdf = "http://ns.adobe.com/pdf/1.3/";
      XNamespace xmp = "http://ns.adobe.com/xap/1.0/";
      XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

      string? GetAlt(XName name) =>
        xdoc.Descendants(name)
          .Descendants(rdf + "li")
          .FirstOrDefault(x => (string?)x.Attribute(XNamespace.Xml + "lang") == "x-default")
          ?.Value
        ?? xdoc.Descendants(name)
          .Descendants(rdf + "li")
          .FirstOrDefault()?.Value;

      string? GetSeq(XName name) {
        var values = xdoc.Descendants(name)
          .Descendants(rdf + "li")
          .Select(x => x.Value)
          .Where(v => !string.IsNullOrWhiteSpace(v))
          .ToList();

        return values.Count > 0 ? string.Join(", ", values) : null;
      }

      AddIfNotEmpty(dict, "Title", GetAlt(dc + "title"));
      AddIfNotEmpty(dict, "Author", GetSeq(dc + "creator"));
      AddIfNotEmpty(dict, "Subject", GetAlt(dc + "description"));

      AddIfNotEmpty(dict, "Keywords",
        xdoc.Descendants(pdf + "Keywords").FirstOrDefault()?.Value);

      AddIfNotEmpty(dict, "Producer",
        xdoc.Descendants(pdf + "Producer").FirstOrDefault()?.Value);

      AddIfNotEmpty(dict, "Creator",
        xdoc.Descendants(xmp + "CreatorTool").FirstOrDefault()?.Value);
    } catch { /* ignored */ }
  }

  private static void AddIfNotEmpty(IDictionary<string, string> dict, string key, string? value) {
    if (!string.IsNullOrWhiteSpace(value) &&
        MetaDataKeysOfInterest.Contains(key)) {
      dict[key] = value.Trim();
    }
  }
}
