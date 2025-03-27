using Flink.Themes;
using Flink.WebUI;
using Flink.WebUI.Ports.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;

namespace Flink.UnitTests.Themes;

public class ThemeManagerTests {
  private readonly string _webRoot = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Flink", "wwwroot");
  
  [Fact]
  public void ThemeDirectoryConfigured_QueryingAvailableThemes_ReturnsCorrectCount() {
    // Arrange
    var webHostEnvironment = new Mock<IWebHostEnvironment>();
    webHostEnvironment.Setup(x => x.WebRootPath).Returns(_webRoot);
    
    var themeManager = new ThemeManager(webHostEnvironment.Object, Options.Create(new WebUiConfiguration { DefaultTheme = "flink-default"}));
    var dirs = Directory.GetDirectories(Path.Combine(_webRoot, "themes"));
    // Act
    var themes = themeManager.AvailableThemes();
    
    // Assert
    themes.Length.Should().Be(dirs.Length);
    themes.Should().Contain("flink-default");
  }
  
  [Fact]
  public void FlinkDefaultThemeIsAvailable_QueryingIndexForTheme_ReturnsTheme() {
    // Arrange
    var webHostEnvironment = new Mock<IWebHostEnvironment>();
    webHostEnvironment.Setup(x => x.WebRootPath).Returns(_webRoot);
    
    var themeManager = new ThemeManager(webHostEnvironment.Object, Options.Create(new WebUiConfiguration { DefaultTheme = "flink-default"}));

    // Act
    var theme = themeManager.GetFile("index.html", "flink-default");
    
    // Assert
    theme.Should().NotBeNull();
  }
}
