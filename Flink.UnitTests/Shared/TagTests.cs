using Flink.Shared;
using FluentAssertions;

namespace Flink.UnitTests.Shared;

public class TagTests {
  [Theory]
  [InlineData(null)]
  [InlineData(0)]
  [InlineData("")]
  public void KeyAndValue_CreateTag_ReturnsSetKeyAndValue(object? v) {
    // Act
    var kvp = Tag.Create("k", v);
    
    // Assert
    kvp.Key.Should().Be("k");
    kvp.Value.Should().Be(v);
  }
}
