using Flink.Embed;
using Microsoft.AspNetCore.Mvc;

namespace Flink.WebUI.Ports.Controllers;

[ApiController]
[Produces("text/html")]
public class WebUiController(IWebHostEnvironment webHostEnvironment) : ControllerBase {
  private static readonly EmbedHtmlBuilder _404Embed = new EmbedHtmlBuilder()
    .WithTitle("404 / Not Found")
    .WithImage("/static/flink-logo.png", "flink logo")
    .WithDescription("The requested ShortLink does not exist");

  private static readonly EmbedHtmlBuilder _500Embed = new EmbedHtmlBuilder()
    .WithTitle("Temporary Server Error")
    .WithImage("/static/flink-logo.png", "flink logo")
    .WithDescription("Flink is experiencing a problem. The link preview did not work");
    
  [HttpGet("/")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public IActionResult GetIndex() {
    var body = System.IO.File.ReadAllText($"{webHostEnvironment.WebRootPath}/index.html");
    return Content(body, "text/html");
  }
}
