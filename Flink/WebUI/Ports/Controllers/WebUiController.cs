using Microsoft.AspNetCore.Mvc;

namespace Flink.WebUI.Ports.Controllers;

[ApiController]
[Produces("text/html")]
public class WebUiController(IWebHostEnvironment webHostEnvironment) : ControllerBase {
  [HttpGet("/")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult GetIndex() {
    var body = System.IO.File.ReadAllText($"{webHostEnvironment.WebRootPath}/index.html");
    return Content(body, "text/html");
  }
}
