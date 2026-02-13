using iText.Kernel.Pdf;
using iText.Kernel.XMP;

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
      "ModDate"
    };

  public static IDictionary<string, string> ExtractMetadata(byte[] pdfBytes) {
    try {
      using var stream = new MemoryStream(pdfBytes);
      using var reader = new PdfReader(stream);
      using var pdfDoc = new PdfDocument(reader);

      var metaDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      ExtractInfoDictionary(pdfDoc, metaDict);
      ExtractXmpMetadata(pdfDoc, metaDict);

      metaDict.TryAdd("Title", "Unknown");

      return metaDict;
    } catch {
      return new Dictionary<string, string>();
    }
  }

  private static void ExtractInfoDictionary(PdfDocument pdfDoc, IDictionary<string, string> dict) {
    try {
      var info = pdfDoc.GetDocumentInfo();

      AddIfNotEmpty(dict, "title", info.GetTitle());
      AddIfNotEmpty(dict, "author", info.GetAuthor());
      AddIfNotEmpty(dict, "description", info.GetSubject());
      AddIfNotEmpty(dict, "keywords", info.GetKeywords());
      AddIfNotEmpty(dict, "creator", info.GetCreator());
      AddIfNotEmpty(dict, "producer", info.GetProducer());
    } catch {
      // ignored
    }
  }

  private static void ExtractXmpMetadata(PdfDocument pdfDoc, IDictionary<string, string> dict) {
    try {
      var xmpMeta = pdfDoc.GetXmpMetadata();
      if (xmpMeta == null)
        return;

      AddIfNotEmpty(dict, "title",
        GetLocalizedXmpProperty(xmpMeta, XMPConst.NS_DC, "title"));
      AddIfNotEmpty(dict, "author",
        GetArrayXmpProperty(xmpMeta, XMPConst.NS_DC, "creator"));
      AddIfNotEmpty(dict, "description",
        GetLocalizedXmpProperty(xmpMeta, XMPConst.NS_DC, "description"));
      AddIfNotEmpty(dict, "Keywords",
        xmpMeta.GetPropertyString(XMPConst.NS_PDF, "Keywords"));
      AddIfNotEmpty(dict, "Producer",
        xmpMeta.GetPropertyString(XMPConst.NS_PDF, "Producer"));
      AddIfNotEmpty(dict, "Creator",
        xmpMeta.GetPropertyString(XMPConst.NS_XMP, "CreatorTool"));
    } catch {
      // ignored
    }
  }

  private static string? GetLocalizedXmpProperty(XMPMeta meta, string ns, string prop) {
    try {
      var value = meta.GetLocalizedText(ns, prop, null, "x-default");
      return value?.GetValue();
    } catch {
      return null;
    }
  }

  private static string? GetArrayXmpProperty(XMPMeta meta, string ns, string prop) {
    try {
      var count = meta.CountArrayItems(ns, prop);
      if (count == 0)
        return null;

      var values = new List<string>();
      for (var i = 1; i <= count; i++) {
        var item = meta.GetArrayItem(ns, prop, i);
        if (!string.IsNullOrWhiteSpace(item?.GetValue()))
          values.Add(item.GetValue());
      }

      return values.Count > 0
        ? string.Join(", ", values)
        : null;
    } catch {
      return null;
    }
  }

  private static void AddIfNotEmpty(IDictionary<string, string> dict, string key, string? value) {
    if (!string.IsNullOrWhiteSpace(value) &&
        MetaDataKeysOfInterest.Contains(key)) {
      dict[key] = value.Trim();
    }
  }
}
