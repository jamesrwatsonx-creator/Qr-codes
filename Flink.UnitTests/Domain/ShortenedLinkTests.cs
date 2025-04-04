using Flink.Domain;

namespace Flink.UnitTests.Domain;

public class ShortenedLinkTests {
  [Fact]
  public Task Verify_NewShortenedLink_Matches() {
    var shortenedLink = new ShortenedLink {
      Creator = "raffael",
      CreationDate = DateTime.MinValue.AddYears(2000).AddDays(2),
      Id = "a-super-unique-id-that-is-awesome",
      Uri = "https://rtrace.io"
    };

    return Verify(shortenedLink);
  }
}
