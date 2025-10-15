using Flink.WebUI.ActionFilter;
using Flink.WebUI.Ports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Flink.WebUI.Ports.Controllers;

[ApiController]
[Produces("text/html")]
[WebUiSecurityHeadersActionFilter]
public class WebUiController(IOptions<WebUiConfiguration> webUiConfig, IThemeManager themeManager) : ControllerBase {
  [HttpGet("/")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult GetIndex([FromQuery] string? theme) {
    var themePath = theme ?? webUiConfig.Value.DefaultTheme;
    if (!themeManager.AvailableThemes().Contains(themePath))
      return NotFound();

    return Content(themeManager.GetFile("index.html", themePath), "text/html");
  }

  [HttpGet("/stats")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult GetStats([FromQuery] string? theme) {
    var themePath = theme ?? webUiConfig.Value.DefaultTheme;
    if (!themeManager.AvailableThemes().Contains(themePath))
      return NotFound();

    return Content(themeManager.GetFile("stats.html", themePath), "text/html");
  }

  [HttpGet("/pw/{shortLinkId}")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult GetPasswordPage(string shortLinkId, [FromQuery] string? theme) {
    var themePath = theme ?? webUiConfig.Value.DefaultTheme;
    if (!themeManager.AvailableThemes().Contains(themePath))
      return NotFound();

    return Content(themeManager.GetFile("password.html", themePath), "text/html");
  }

  [HttpGet("/cd/{shortLinkId}")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult GetCountdown(string shortLinkId, [FromQuery] string? theme) {
    var themePath = theme ?? webUiConfig.Value.DefaultTheme;
    if (!themeManager.AvailableThemes().Contains(themePath))
      return NotFound();

    return Content(themeManager.GetFile("countdown.html", themePath), "text/html");
  }

  [HttpGet("/history")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult GetLocalHistory([FromQuery] string? theme) {
    var themePath = theme ?? webUiConfig.Value.DefaultTheme;
    if (!themeManager.AvailableThemes().Contains(themePath))
      return NotFound();

    return Content(themeManager.GetFile("history.html", themePath), "text/html");
  }
}
