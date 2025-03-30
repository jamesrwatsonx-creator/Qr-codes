using Flink.WebUI;
using Flink.WebUI.Ports.Models;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace Flink.UnitTests.WebUi;

public class InMemoryCachingThemeManagerTests {
  private static IOptions<WebUiConfiguration> GetConfig(string theme = "flink-default", int cacheDuration = 5) {
    return Options.Create(new WebUiConfiguration { ThemeCacheDuration = cacheDuration, DefaultTheme = theme });
  }


  [Fact]
  public void PreviouslyUncachedTheme_QueryThemeIndex_ReturnsUncachedTheme() {
    // Arrange
    var tm = new Mock<IThemeManager>();
    tm.Setup(t => t.AvailableThemes()).Returns(["flink-default"]);
    tm.Setup(t => t.GetFile("index.html", "flink-default")).Returns("something");
    var cachingThemeManager = new InMemoryCachingThemeManager(new MemoryCache(new MemoryCacheOptions()), tm.Object, GetConfig());

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
    var cachingThemeManager = new InMemoryCachingThemeManager(new MemoryCache(new MemoryCacheOptions()), tm.Object, GetConfig());

    cachingThemeManager.GetFile("index.html", "flink-default"); // Cache original
    tm.Setup(t => t.GetFile("index.html", "flink-default")).Throws<Exception>();

    // Act
    var file = cachingThemeManager.GetFile("index.html", "flink-default");

    // Assert
    file.Should().Be("attempt1");
  }

  [Fact]
  public void ZeroCacheDurationConfigured_QueryThemeIndexTwice_NeverReturnsCached() {
    // Arrange
    var tm = new Mock<IThemeManager>();
    tm.Setup(t => t.AvailableThemes()).Returns(["flink-default"]);
    tm.Setup(t => t.GetFile("index.html", "flink-default")).Returns("attempt1");
    var cachingThemeManager = new InMemoryCachingThemeManager(new MemoryCache(new MemoryCacheOptions()), tm.Object, GetConfig(cacheDuration: 0));

    // Act
    cachingThemeManager.GetFile("index.html", "flink-default");
    cachingThemeManager.GetFile("index.html", "flink-default");

    // Assert
    tm.Verify(x => x.GetFile("index.html", "flink-default"), Times.Exactly(2));
  }
}
