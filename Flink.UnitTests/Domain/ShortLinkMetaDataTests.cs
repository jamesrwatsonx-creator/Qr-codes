using Flink.Domain;

namespace Flink.UnitTests.Domain;

public class ShortLinkMetaDataTests {
  [Fact]
  public Task Verify_NewShortenedLink_Matches() {
    var meta = new ShortLinkMetaData() {
      Id = Guid.Empty.ToString(),
      Author = "ruffy",
      Description = "awesome pawesome",
      Image = "awesomeimage",
      Title = "awesome title",
      DocumentSize = 3403030,
      LastUpdate = 3,
      HttpStatusCode = 200
    };

    return Verify(meta);
  }
}
