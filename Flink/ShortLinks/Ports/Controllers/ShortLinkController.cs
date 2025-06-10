using System.Net;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using Flink.Domain;
using Flink.MetaData.Ports;
using Flink.Metrics.Ports.Models;
using Flink.Shared;
using Flink.Shared.Extensions;
using Flink.ShortLinks.Models;
using Flink.SpamProtection.Ports.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Flink.ShortLinks.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class ShortLinkController(
  IShortLinkRepository shortLinkRepository,
  ILogger<ShortLinkController> logger,
  IScrapingQueue scrapingQueue,
  IForbiddenDomainRepository forbiddenDomainRepository,
  ShortLinkMetricsRegistry metrics,
  IOptions<MetricsConfiguration> metricsConfiguration,
  TimeProvider timeProvider)
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
    "stats",
    "spam-protection",
    "pw",
    metricsConfiguration.Value.MetricsEndpointPath.Replace("/", "")
  ];

  /// <summary>
  ///   Get redirected to the URL of the shortLink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <param name="password">password to visit a shortlink (only needed if a shortlink was set up with a password)</param>
  /// <returns></returns>
  [HttpGet("/{shortLinkId}", Name = "GetShortLinkRoot")]
  [HttpGet("/links/{shortLinkId}", Name = "GetShortLink")]
  [ProducesResponseType((int)HttpStatusCode.Redirect)]
  [ProducesResponseType((int)HttpStatusCode.NotFound)]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  public Task<ActionResult> Get(string shortLinkId, [FromQuery] string? password) {
    logger.LogDebug("Requesting Short Link with {Id}", shortLinkId);

    var now = timeProvider.GetUtcNow().ToUnixTimeSeconds();
    return shortLinkRepository
      .GetAsync(shortLinkId)
      .TapError(e => logger.LogError("Could not read Shortened Link with {Id}, because {Error}", shortLinkId, e))
      .Match(maybeLink => maybeLink
        .TapSome(_ => metrics.Ok(shortLinkId))
        .TapNone(() => metrics.NotFound(shortLinkId))
        .GetValueOrDefault(l => GetActionResultForLinkRequest(l, password), NotFound()), _ => Problem());
  }

  /// <summary>
  ///   Create a new flinkified shortlink
  /// </summary>
  /// <param name="shortLinkRequest">information about the new shortlink</param>
  /// <returns>the shortened link</returns>
  [HttpPost("/links", Name = "CreateNewShortLink")]
  [ProducesResponseType(typeof(ShortenedLink), 201, "application/json")]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  public Task<ActionResult> Add(ShortLinkRequest shortLinkRequest) {
    var sanitizedId = shortLinkRequest.PreferredId?.Trim().ToLower();
    if (sanitizedId != null && _forbiddenShortLinkIds.Any(forbiddenShortLinkId => sanitizedId == forbiddenShortLinkId))
      return Task.FromResult<ActionResult>(BadRequest("Reserved ShortLinkId must not be used"));
    if (forbiddenDomainRepository.ContainsDomain(new Uri(shortLinkRequest.Uri).Host))
      return Task.FromResult<ActionResult>(BadRequest("The given URL is considered spam."));

    var actualId = shortLinkRequest.PreferredId?.Trim() ?? ShortLinkIdGenerator.CreateRandomId(shortLinkRequest.PreferredIdLength);
    var shortenedLink = new ShortenedLink {
      Id = actualId,
      Creator = shortLinkRequest.Creator,
      Uri = shortLinkRequest.Uri,
      CreationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
      ActiveFrom = shortLinkRequest.ActiveFrom?.ToUnixTimeSeconds() ?? 0,
      ActiveUntil = shortLinkRequest.ActiveUntil?.ToUnixTimeSeconds() ?? long.MaxValue,
      PasswordHash = shortLinkRequest.Password == null ? null : PasswordHasher.CalculatePasswordHash(shortLinkRequest.Password, actualId)
    };

    return shortLinkRepository
      .AddAsync(shortenedLink)
      .TapError(e => logger.LogError("Could not create Shortened Link with {Id}, because {Error}", shortenedLink.Id, e))
      .Tap(shortLink => logger.LogInformation("Saved Shortened Link with {Id}", shortLink.Id))
      .Check(_ => scrapingQueue.EnqueueForScraping(shortenedLink))
      .Tap(_ => metrics.Created())
      .TapError(_ => metrics.AlreadyExists(shortenedLink.Id))
      .Match(ActionResult (shortLink) => Created($"/{shortLink.Id}", StripPassword(shortLink)), BadRequest);
  }

  /// <summary>
  ///   Delete a (previously flinkified) shortlink
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

  private ActionResult GetActionResultForLinkRequest(ShortenedLink shortenedLink, string? clientProvidedPassword) {
    var now = timeProvider.GetUtcNow().ToUnixTimeSeconds();

    if (now < shortenedLink.ActiveFrom)
      return Redirect($"/cd/{shortenedLink.Id}?activeFrom={shortenedLink.ActiveFrom}");
    
    // No password provided (even though a password is needed) -> Unauthorized
    if (shortenedLink.PasswordHash != null && clientProvidedPassword == null)
      return Redirect($"/pw/{shortenedLink.Id}");
    
    if (now >= shortenedLink.ActiveUntil)
      return NotFound();

    // Short Link is password protected and provided password does not match
    if (shortenedLink.PasswordHash != null
        && clientProvidedPassword != null
        && !PasswordHasher.VerifyPasswordHash(shortenedLink.PasswordHash, clientProvidedPassword, shortenedLink.Id))
      return Unauthorized();

    return Redirect(shortenedLink.Uri);
  }

  private static ShortenedLink StripPassword(ShortenedLink shortenedLink) => new() {
    Id = shortenedLink.Id,
    Uri = shortenedLink.Uri,
    Creator = shortenedLink.Creator,
    CreationDate = shortenedLink.CreationDate,
    ActiveFrom = shortenedLink.ActiveFrom,
    ActiveUntil = shortenedLink.ActiveUntil,
    PasswordHash = null
  };
}
