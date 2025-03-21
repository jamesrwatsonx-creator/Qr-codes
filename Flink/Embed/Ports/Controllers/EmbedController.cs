using CSharpFunctionalExtensions;
using Flink.MetaData.Repositories;
using Flink.ShortLinks.Ports;
using Flink.WebUI.Ports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Flink.Embed.Ports.Controllers;

[ApiController]
[Produces("text/html")]
public class EmbedController(IOptions<WebUiConfiguration> webUiConfig, IMetaDataRepository metaDataRepository, IShortLinkRepository shortLinkRepository) : ControllerBase {
  private readonly EmbedHtmlBuilder _404Embed = new EmbedHtmlBuilder(webUiConfig.Value.DefaultTheme)
    .WithTitle("404 / Not Found")
    .WithImage("/static/img/flink-logo.png", "flink logo")
    .WithDescription("The requested ShortLink does not exist");

  private readonly EmbedHtmlBuilder _500Embed = new EmbedHtmlBuilder(webUiConfig.Value.DefaultTheme)
    .WithTitle("Temporary Server Error")
    .WithImage("/static/img/flink-logo.png", "flink logo")
    .WithDescription("Flink is experiencing a problem. The link preview did not work");

  /// <summary>
  /// Render a Link Preview for any (previously created) shortlink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <returns></returns>
  [HttpGet("/links/{shortLinkId}/embed", Name = "GetEmbed")]
  [Produces("text/html")]
  public async Task<IActionResult> GetEmbed(string shortLinkId) {
    var shortLinkResult = await shortLinkRepository.GetAsync(shortLinkId);
    if (shortLinkResult.IsFailure) return Content(_500Embed.Build(), "text/html");
    if (shortLinkResult is { IsSuccess: true, Value.HasNoValue: true }) return Content(_404Embed.Build(), "text/html");
    var metaDataResult = await metaDataRepository.GetAsync(shortLinkId);
    if (metaDataResult.IsFailure) return Content(_500Embed.Build(), "text/html");
    if (metaDataResult is { IsSuccess: true, Value.HasNoValue: true }) return Content(_404Embed.Build(), "text/html");
    
    
    var embedHtml = metaDataResult.Value
      .ToResult(string.Empty)
      .Match(v => new EmbedHtmlBuilder(webUiConfig.Value.DefaultTheme)
        .WithTitle(v.Title)
        .WithDescription(v.Description)
        .WithShortLinkId(shortLinkResult.GetValueOrDefault().GetValueOrDefault()?.Id!)
        .WithImage($"/links/{v.Id}/thumbnail", v.Title)
        .WithOriginalUrl(shortLinkResult.GetValueOrDefault().GetValueOrDefault()?.Uri)
        .Build(), _ => _404Embed.Build());

    return Content(embedHtml, "text/html");
  }
}
