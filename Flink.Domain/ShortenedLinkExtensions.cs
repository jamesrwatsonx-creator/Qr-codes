namespace Flink.Domain;

public static class ShortenedLinkExtensions {
  extension(ShortenedLink shortenedLink) {
    public ShortenedLink StripPassword() => new() {
      Id = shortenedLink.Id,
      Uri = shortenedLink.Uri,
      Creator = shortenedLink.Creator,
      CreationDate = shortenedLink.CreationDate,
      ActiveFrom = shortenedLink.ActiveFrom,
      ActiveUntil = shortenedLink.ActiveUntil,
      PasswordHash = null
    };

    public ShortenedLink StripUri() => new() {
      Id = shortenedLink.Id,
      Uri = "stripped",
      Creator = shortenedLink.Creator,
      CreationDate = shortenedLink.CreationDate,
      ActiveFrom = shortenedLink.ActiveFrom,
      ActiveUntil = shortenedLink.ActiveUntil,
      PasswordHash = shortenedLink.PasswordHash
    };
  }
}
