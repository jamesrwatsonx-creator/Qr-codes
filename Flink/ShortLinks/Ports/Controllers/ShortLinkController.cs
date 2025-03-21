using System.Net;
using Microsoft.AspNetCore.Mvc;
using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData.Ports;
using Flink.Metrics.Ports.Models;
using Flink.Shared.Extensions;
using Flink.ShortLinks.Models;
using Microsoft.Extensions.Options;

namespace Flink.ShortLinks.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class ShortLinkController(
  IShortLinkRepository shortLinkRepository,
  ILogger<ShortLinkController> logger,
  IScrapingQueue scrapingQueue,
  ShortLinkMetricsRegistry metrics,
  IOptions<MetricsConfiguration> metricsConfiguration)
  : ControllerBase {

  private readonly string[] _forbiddenShortLinkIds = [
    "",
    " ",
    "metrics",
    "to",
    "scalar",
    "swagger",
    "index",
    "meta",
    "embed",
    "proxy",
    "image",
    "/",
    "index.html",
    "info",
    "image",
    "qr",
    "metadata",
    "embeds",
    "links",
    metricsConfiguration.Value.MetricsEndpointPath,
  ];
  
  /// <summary>
  /// Get redirected to the URL of the shortLink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <returns></returns>
  [HttpGet("/{shortLinkId}", Name="GetShortLinkRoot")]
  [HttpGet("/links/{shortLinkId}", Name = "GetShortLink")]
  [ProducesResponseType((int)HttpStatusCode.Redirect)]
  [ProducesResponseType((int)HttpStatusCode.NotFound)]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  public Task<ActionResult> Get(string shortLinkId) {
    logger.LogDebug("Requesting Short Link with {Id}", shortLinkId);

    return shortLinkRepository
      .GetAsync(shortLinkId)
      .TapError(e => logger.LogError("Could not read Shortened Link with {Id}, because {Error}", shortLinkId, e))
      .Match(maybeLink => maybeLink
        .TapSome(_ => metrics.Ok(shortLinkId))
        .TapNone(() => metrics.NotFound(shortLinkId))
        .Match(ActionResult (l) => Redirect(l.Uri), NotFound), _ => Problem());
  }

  /// <summary>
  /// Create a new flinkified shortlink
  /// </summary>
  /// <param name="shortLinkRequest">information about the new shortlink</param>
  /// <returns>the shortened link</returns>
  [HttpPost("/links", Name = "CreateNewShortLink")]
  [ProducesResponseType(typeof(ShortenedLink), 201, "application/json")]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  public Task<ActionResult> Add(ShortLinkRequest shortLinkRequest) {
    var sanitizedId = shortLinkRequest.PreferredId?.Trim().ToLower();
    if (sanitizedId != null && _forbiddenShortLinkIds.Any(forbiddenShortLinkId => sanitizedId == forbiddenShortLinkId)) 
      return Task.FromResult<ActionResult>(BadRequest());
 
    var shortenedLink = new ShortenedLink {
      Id = shortLinkRequest.PreferredId?.Trim() ?? ShortLinkIdGenerator.CreateRandomId(shortLinkRequest.PreferredIdLength),
      Creator = shortLinkRequest.Creator,
      Uri = shortLinkRequest.Uri,
      CreationDate = DateTime.UtcNow
    };

    return shortLinkRepository
      .AddAsync(shortenedLink)
      .TapError(e => logger.LogError("Could not create Shortened Link with {Id}, because {Error}", shortenedLink.Id, e))
      .Tap(shortLink => logger.LogInformation("Saved Shortened Link with {Id}", shortLink.Id))
      .Check(_ => scrapingQueue.EnqueueForScraping(shortenedLink))
      .Tap(_ => metrics.Created())
      .TapError(_ => metrics.AlreadyExists(shortenedLink.Id))
      .Match(ActionResult (shortLink) => Created($"/to/{shortLink.Id}", shortLink), BadRequest);
  }

  /// <summary>
  /// Delete a (previously flinkified) shortlink
  /// </summary>
  /// <param name="shortLinkId">Id of the shortlink to remove</param>
  [HttpDelete("/links/{shortLinkId}", Name = "RemoveShortLink")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
  public Task<ActionResult> Delete(string shortLinkId) {
    return shortLinkRepository.DeleteAsync(shortLinkId)
      .Tap(() => logger.LogInformation("Deleted Shortened Link {Id}", shortLinkId))
      .TapError(f => logger.LogError("Could not delete Shortened Link {Id}, because {Error}", shortLinkId, f))
      .Match(Ok, ActionResult (f) => BadRequest(f));
  }
}
