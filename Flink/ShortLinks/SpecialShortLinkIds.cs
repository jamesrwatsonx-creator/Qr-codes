namespace Flink.ShortLinks;

public static class SpecialShortLinkIds {
  public static readonly HashSet<string> ReservedShortLinkIds = [
    "links",
    "info",
    "spam-protection",
    "webui",
    "pw",
    "password",
    "cd",
    "static",
    "countdown",
    "history",
    "stats",
    "swagger",
    "scalar",
    "metrics",
    "themes"
  ];
}
