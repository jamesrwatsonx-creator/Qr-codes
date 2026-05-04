using System.Security.Cryptography;
using System.Text;
using Flink.SpamProtection.Models;

namespace Flink.SpamProtection.BusinessLogic;

public static class BlockListParser {
  private static string? ReadHeaderField(string[] lines, string fieldName, string? fallback = null) {
    try {
      var relevantLine = lines.SingleOrDefault(l => l.StartsWith('!') && l.Contains(':') && l.ToLowerInvariant().Contains(fieldName.ToLowerInvariant()));
      if (relevantLine == null) return fallback;

      var index = relevantLine.IndexOf(':');
      return index == -1
        ? fallback
        : relevantLine[(index + 1)..].Trim();
    } catch {
      return fallback;
    }
  }

  public static BlockListSource? ParseBlockListSourceInformation(string source, string blockListTxt) {
    try {
      var lines = blockListTxt
        .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(l => l.StartsWith('!'))
        .ToArray();

      var title = ReadHeaderField(lines, "Title", source)!;
      var version = ReadHeaderField(lines, "Version", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())!;
      var metaData = new Dictionary<string, string?> {
          { "Homepage", ReadHeaderField(lines, "Homepage") },
          { "License", ReadHeaderField(lines, "License", ReadHeaderField(lines, "Licence")) },
          { "Maintainers", ReadHeaderField(lines, "Maintainers", ReadHeaderField(lines, "Maintainer")) },
          { "Repository", ReadHeaderField(lines, "Repository") },
          { "Expires", ReadHeaderField(lines, "Expires") },
          { "LastModified", ReadHeaderField(lines, "Last modified") },
          { "Commit", ReadHeaderField(lines, "Commit") },
          { "Author", ReadHeaderField(lines, "Author") },
        }
        .Where(kv => kv.Value != null)
        .ToDictionary(kv => kv.Key, kv => kv.Value!);

      return new BlockListSource {
        Title = title,
        Source = source,
        Version = Convert.ToUInt64(version),
        Sha3 = Convert.ToBase64String(SHA3_512.HashData(Encoding.UTF8.GetBytes(blockListTxt))),
        MetaData = metaData
      };
    } catch (Exception) {
      return null;
    }
  }

  public static IEnumerable<string> ParseExactMatches(string blockListText) {
    if (string.IsNullOrWhiteSpace(blockListText))
      yield break;

    var lines = blockListText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    foreach (var raw in lines) {
      if (string.IsNullOrEmpty(raw))
        continue;
      
      if (raw[0] == '!' || raw.StartsWith("@@"))
        continue;
      
      if (raw.Contains("##") || raw.Contains("#?#"))
        continue;
      
      if (!raw.StartsWith("||") || !raw.EndsWith('^') || raw.Contains('*'))
        continue;
      
      if (raw.Contains('$')) {
        if (ContainsAnyDisallowedModifier(raw))
          continue;
      }
      
      var hostSpan = raw.AsSpan(2, raw.Length - 3).Trim();

      if (hostSpan.IsEmpty)
        continue;
      
      var host = hostSpan.ToString().ToLowerInvariant();

      yield return host;
    }
  }

  private static bool ContainsAnyDisallowedModifier(string raw) {
    // single scan, no repeated Contains chains
    return raw.Contains("$~") ||
           raw.Contains("$script") ||
           raw.Contains("$document") ||
           raw.Contains("$third-party") ||
           raw.Contains("$popup") ||
           raw.Contains("$subdocument") ||
           raw.Contains("$stylesheet") ||
           raw.Contains("$xmlhttprequest") ||
           raw.Contains("$object") ||
           raw.Contains("$image") ||
           raw.Contains("$domain=") ||
           raw.Contains("$media") ||
           raw.Contains("$doc") ||
           raw.Contains("$all");
  }
}
