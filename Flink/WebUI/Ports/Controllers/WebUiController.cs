using Microsoft.AspNetCore.Mvc;

[ApiController]
[Produces("text/html")]
public class WebUiController(IWebHostEnvironment webHostEnvironment) : ControllerBase {
  [HttpGet("/")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult Get() {
    var body = System.IO.File.ReadAllText($"{webHostEnvironment.WebRootPath}/index.html");
    return Content(body, "text/html");
  }
}
