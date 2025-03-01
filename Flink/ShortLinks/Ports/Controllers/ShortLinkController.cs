using System.Net;
using Microsoft.AspNetCore.Mvc;
using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData.Ports;
using Flink.Shared.Extensions;
using Flink.ShortLinks.Models;

namespace Flink.ShortLinks.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class ShortLinkController(
  IShortLinkRepository shortLinkRepository,
  ILogger<ShortLinkController> logger,
  IScrapingQueue scrapingQueue,
  ShortLinkMetricsRegistry metrics)
  : ControllerBase {
  
  /// <summary>
  /// Get redirected to the URL of the ShortLink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <returns></returns>
  [HttpGet("/to/{shortLinkId}", Name = "GetShortLink")]
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
  [HttpPost("/to", Name = "CreateNewShortLink")]
  [ProducesResponseType(typeof(ShortenedLink), 201, "application/json")]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  public Task<ActionResult> Add(ShortLinkRequest shortLinkRequest) {
    var shortenedLink = new ShortenedLink {
      Id = shortLinkRequest.PreferredId ?? ShortLinkIdGenerator.CreateRandomId(shortLinkRequest.PreferredIdLength),
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
}
