using Flink.WebUI.Ports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Flink.WebUI.Ports.Controllers;

[ApiController]
[Produces("text/html")]
public class WebUiController(IWebHostEnvironment webHostEnvironment, IOptions<WebUiConfiguration> webUiConfig) : ControllerBase {
  [HttpGet("/")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult GetIndex() {
    var body = System.IO.File.ReadAllText($"{webHostEnvironment.WebRootPath}/themes/{webUiConfig.Value.DefaultTheme}/index.html");
    return Content(body, "text/html");
  }
}
