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
        : relevantLine[(index+1)..].Trim();
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
    return blockListText
      .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
      .Where(l => !l.StartsWith('!'))
      .Where(l => !l.StartsWith("@@"))
      .Where(l => !l.Contains("##"))
      .Where(l => !l.Contains("#?#"))
      .Where(l => !l.Contains("##"))
      .Where(l => !l.Contains("$~"))
      .Where(l => !l.Contains("$script"))
      .Where(l => !l.Contains("$document"))
      .Where(l => !l.Contains("$third-party"))
      .Where(l => !l.Contains("$popup"))
      .Where(l => !l.Contains("$subdocument"))
      .Where(l => !l.Contains("$stylesheet"))
      .Where(l => !l.Contains("$xmlhttprequest"))
      .Where(l => !l.Contains("$object"))
      .Where(l => !l.Contains("$image"))
      .Where(l => !l.Contains("$domain="))
      .Where(l => !l.Contains("$media"))
      .Select(l => l.Replace("$all", ""))
      .Select(l => l.Replace("$doc", ""))
      .Where(l => l.StartsWith("||") && l.EndsWith('^') && !l.Contains('*'))
      .Select(l => l[2..^1].Trim().ToLowerInvariant())
      .Where(l => !string.IsNullOrWhiteSpace(l))
      .ToArray();
  }
}
