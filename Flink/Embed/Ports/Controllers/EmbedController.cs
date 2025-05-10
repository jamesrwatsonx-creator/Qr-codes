using Flink.Domain;
using Flink.Embed.ActionFilter;
using Flink.MetaData.Repositories;
using Flink.ShortLinks.Ports;
using Flink.WebUI.Ports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Flink.Embed.Ports.Controllers;

[ApiController]
[Produces("text/html")]
[EmbedSecurityHeadersActionFilter]
public class EmbedController(
  IOptions<WebUiConfiguration> webUiConfig,
  IMetaDataRepository metaDataRepository,
  IShortLinkRepository shortLinkRepository,
  EmbedMetricsRegistry metricsRegistry)
  : ControllerBase {
  private readonly string _404Embed = new EmbedHtmlBuilder(webUiConfig.Value.DefaultTheme)
    .WithTitle("404 / Not Found")
    .WithImage("/static/img/flink-logo.png", "flink logo")
    .WithDescription("The requested ShortLink does not exist")
    .Build();

  private readonly string _500Embed = new EmbedHtmlBuilder(webUiConfig.Value.DefaultTheme)
    .WithTitle("Temporary Server Error")
    .WithImage("/static/img/flink-logo.png", "flink logo")
    .WithDescription("Flink is experiencing a problem. The link preview did not work")
    .Build();

  /// <summary>
  ///   Render a Link Preview for any (previously created) shortlink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <returns></returns>
  [HttpGet("/links/{shortLinkId}/embed", Name = "GetEmbed")]
  [Produces("text/html")]
  public async Task<IActionResult> GetEmbed(string shortLinkId) {
    var shortLinkResult = await shortLinkRepository.GetAsync(shortLinkId);
    if (shortLinkResult.IsFailure)
      return Content(_500Embed, "text/html");
    if (shortLinkResult is { IsSuccess: true, Value.HasNoValue: true })
      return Content(_404Embed, "text/html");

    var metaDataResult = await metaDataRepository.GetAsync(shortLinkId);
    if (metaDataResult.IsFailure)
      return Content(_500Embed, "text/html");

    if (metaDataResult is { IsSuccess: true, Value.HasNoValue: true })
      return Content(_404Embed, "text/html");

    metricsRegistry.Success(shortLinkId);
    var embedHtml = BuildEmbed(metaDataResult.Value.Value, shortLinkResult.Value.Value);
    return Content(embedHtml, "text/html");
  }

  private string BuildEmbed(ShortLinkMetaData metaData, ShortenedLink shortLink) {
    return new EmbedHtmlBuilder(webUiConfig.Value.DefaultTheme)
      .WithTitle(metaData.Title)
      .WithDescription(metaData.Description)
      .WithShortLinkId(shortLink.Id)
      .WithImage($"/links/{shortLink.Id}/thumbnail", metaData.Title)
      .WithOriginalUrl(shortLink.Uri)
      .Build();
  }
}
