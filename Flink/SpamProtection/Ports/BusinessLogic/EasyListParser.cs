using CSharpFunctionalExtensions;
using Flink.SpamProtection.Models;

namespace Flink.SpamProtection.Ports.BusinessLogic;

public static class EasyListParser {
  public static Result<EasyListSource> ParseHeader(string source, string easyListTxt) {
    try {
      var headerLines = easyListTxt
        .Split('\n')
        .SkipWhile(l => !l.StartsWith("! "))
        .TakeWhile(l => l.StartsWith("! "))
        .ToArray();

      var title = headerLines
        .Single(l => l.StartsWith("! Title: ")).Replace("! Title: ", "");
      var version = headerLines
        .FirstOrDefault(l => l.StartsWith("! Version: "))?.Replace("! Version: ", "") ?? "0";
      var lastModifiedString = headerLines
                                 .FirstOrDefault(l => l.StartsWith("! Last modified: "))?
                                 .Replace("! Last modified: ", "")?
                                 .Replace("UTC", "")?
                                 .Trim()
                               ?? headerLines
                                 .FirstOrDefault(l => l.StartsWith("! Updated: "))?
                                 .Replace("! Updated: ", "")?
                                 .Trim();

      var lastModifiedDateTime = lastModifiedString != null
        ? DateTime.Parse(lastModifiedString)
        : DateTime.UtcNow;
      return Result.Success(new EasyListSource {
        Title = title,
        Source = source,
        Version = Convert.ToUInt64(version),
        LastModified = lastModifiedDateTime
      });
    } catch (Exception exc) {
      return Result.Failure<EasyListSource>(exc.Message);
    }
  }

  public static string[] ParseForbiddenDomains(string easyListTxt) {
    var domainLines = easyListTxt.Split('\n')
      .Where(l => !l.StartsWith("! ")) // ignore Comments
      .Where(l => l.StartsWith("||") && l.EndsWith('^')) // filter for lines with Domain Name anchors
      .Select(l => l.Replace("||", "").Replace("^", "")) // extract domain string
      .ToArray();
    return domainLines;
  }
}
