using Flink.Themes;
using Flink.WebUI.Ports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Flink.WebUI.Ports.Controllers;

[ApiController]
[Produces("text/html")]
public class WebUiController(IOptions<WebUiConfiguration> webUiConfig, IThemeManager themeManager) : ControllerBase {
  [HttpGet("/")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult GetIndex([FromQuery] string? theme) {
    var themePath = theme ?? webUiConfig.Value.DefaultTheme;
    if (!themeManager.AvailableThemes().Contains(themePath))
      return NotFound();
    
    return Content(themeManager.GetFile("index.html", themePath), "text/html");
  }
}
