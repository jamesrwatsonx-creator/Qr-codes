using Flink.MetaData.Extensions;
using FluentAssertions;

namespace Flink.UnitTests.MetaData;

public class DictionaryExtensionTests {
  [Fact]
  public void EmptyDictionary_GetOrDefaultWithNonExistingKeyIsCalled_ReturnsDefault() {
    // Arrange
    var x = new Dictionary<string, string>();
    // Act
    var v = x.GetOrDefault(["some-non-existing-key"], "default");
    // Assert
    v.Should().Be("default");
  }

  [Fact]
  public void EmptyDictionary_GetOrDefaultWithNoKeysIsCalled_ReturnsDefault() {
    // Arrange
    var x = new Dictionary<string, string>();
    // Act
    var v = x.GetOrDefault([], "default");
    // Assert
    v.Should().Be("default");
  }
}
