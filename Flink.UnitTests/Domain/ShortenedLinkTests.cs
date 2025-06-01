using Flink.Domain;

namespace Flink.UnitTests.Domain;

public class ShortenedLinkTests {
  [Fact]
  public Task Verify_NewShortenedLink_Matches() {
    var shortenedLink = new ShortenedLink {
      Creator = "raffael",
      CreationDate = 1,
      Id = "a-super-unique-id-that-is-awesome",
      Uri = "https://rtrace.io",
      ActiveFrom = 1,
      ActiveUntil = 1
    };

    return Verify(shortenedLink);
  }
}
