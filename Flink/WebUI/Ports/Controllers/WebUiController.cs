using CSharpFunctionalExtensions;
using Flink.Embed;
using Flink.MetaData.Repositories;
using Flink.ShortLinks.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Flink.WebUI.Ports.Controllers;

[ApiController]
[Produces("text/html")]
public class WebUiController(IWebHostEnvironment webHostEnvironment,
  IMetaDataRepository metaDataRepository,
  IShortLinkRepository shortLinkRepository) : ControllerBase {
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
  
  [HttpGet("/embed/{id}")]
  public async Task<IActionResult> GetEmbed(string id) {
    var shortLinkResult = await shortLinkRepository.GetAsync(id);
    if (shortLinkResult.IsFailure) return Content(_500Embed.Build(), "text/html");
    if (shortLinkResult is { IsSuccess: true, Value.HasNoValue: true }) return Content(_404Embed.Build(), "text/html");
    var metaDataResult = await metaDataRepository.GetAsync(id);
    if (metaDataResult.IsFailure) return Content(_500Embed.Build(), "text/html");
    if (metaDataResult is { IsSuccess: true, Value.HasNoValue: true }) return Content(_404Embed.Build(), "text/html");
    
    var embedHtml = metaDataResult.Value
      .ToResult(string.Empty)
      .Match(v => new EmbedHtmlBuilder()
        .WithTitle(v.Title)
        .WithDescription(v.Description)
        .WithShortLinkId(shortLinkResult.GetValueOrDefault().GetValueOrDefault()?.Id!)
        .WithImage(v.Image, v.Title)
        .WithOriginalUrl(shortLinkResult.GetValueOrDefault().GetValueOrDefault()?.Uri)
        .Build(), _ => _404Embed.Build());
    
    return Content(embedHtml, "text/html");
  }
}
