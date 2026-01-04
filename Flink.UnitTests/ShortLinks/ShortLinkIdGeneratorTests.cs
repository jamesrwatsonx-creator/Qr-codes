using Flink.ShortLinks;
using FluentAssertions;

namespace Flink.UnitTests.ShortLinks;

public class ShortLinkIdGeneratorTests {
  [Theory]
  [InlineData(128)]
  [InlineData(1)]
  [InlineData(0)]
  public void ShortLinkGeneratorCalled_WithFixedLength_ReturnsCorrectLength(int length) {
    var id = ShortLinkIdGenerator.CreateRandomId(length);
    id.Should().NotBeNull();
    id.Length.Should().Be(length);
  }

  [Fact]
  public void ShortLinkGeneratorCalled_WithFixedLengthTwice_ReturnsDifferentId() {
    var id1 = ShortLinkIdGenerator.CreateRandomId(12);
    var id2 = ShortLinkIdGenerator.CreateRandomId(12);
    id1.Should().NotBeSameAs(id2);
  }
}
