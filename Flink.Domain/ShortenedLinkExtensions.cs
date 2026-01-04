namespace Flink.Domain;

public static class ShortenedLinkExtensions {
  public static ShortenedLink StripPassword(this ShortenedLink shortenedLink) => new() {
    Id = shortenedLink.Id,
    Uri = shortenedLink.Uri,
    Creator = shortenedLink.Creator,
    CreationDate = shortenedLink.CreationDate,
    ActiveFrom = shortenedLink.ActiveFrom,
    ActiveUntil = shortenedLink.ActiveUntil,
    PasswordHash = null
  };
  
  public static ShortenedLink StripUri(this ShortenedLink shortenedLink) => new() {
    Id = shortenedLink.Id,
    Uri = "stripped",
    Creator = shortenedLink.Creator,
    CreationDate = shortenedLink.CreationDate,
    ActiveFrom = shortenedLink.ActiveFrom,
    ActiveUntil = shortenedLink.ActiveUntil,
    PasswordHash = shortenedLink.PasswordHash
  };
  
}
