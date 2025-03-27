using Flink.Themes;
using Flink.WebUI;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Flink.UnitTests.Themes;

public class InMemoryCachingThemeManagerTests {
  [Fact]
  public void PreviouslyUncachedTheme_QueryThemeIndex_ReturnsUncachedTheme() {
    // Arrange
    var tm = new Mock<IThemeManager>();
    tm.Setup(t => t.AvailableThemes()).Returns(["flink-default"]);
    tm.Setup(t => t.GetFile("index.html", "flink-default")).Returns("something");
    var cachingThemeManager = new InMemoryCachingThemeManager(new MemoryCache(new MemoryCacheOptions()), tm.Object);
    
    // Act
    var file = cachingThemeManager.GetFile("index.html", "flink-default");
    
    // Assert
    file.Should().Be("something");
  }

  [Fact]
  public void PreviouslyCachedTheme_QueryThemeIndex_ReturnsCachedTheme() {
    // Arrange
    var tm = new Mock<IThemeManager>();
    tm.Setup(t => t.AvailableThemes()).Returns(["flink-default"]);
    tm.Setup(t => t.GetFile("index.html", "flink-default")).Returns("attempt1");
    var cachingThemeManager = new InMemoryCachingThemeManager(new MemoryCache(new MemoryCacheOptions()), tm.Object);
    
    cachingThemeManager.GetFile("index.html", "flink-default"); // Cache original
    tm.Setup(t => t.GetFile("index.html", "flink-default")).Throws<Exception>();
    
    // Act
    var file = cachingThemeManager.GetFile("index.html", "flink-default");
    
    // Assert
    file.Should().Be("attempt1");
  }
}
