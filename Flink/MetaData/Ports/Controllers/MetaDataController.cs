using System.Net;
using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.Embed.Ports;
using Flink.MetaData.Repositories;
using Flink.Shared;
using Flink.ShortLinks.Ports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flink.MetaData.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class MetaDataController(
  IMetaDataRepository metaDataRepository,
  IShortLinkRepository shortLinkRepository,
  ILogger<MetaDataController> logger,
  EmbedMetricsRegistry metricsRegistry)
  : ControllerBase {
  /// <summary>
  ///   Query the crawled/scraped metadata for any given shortlink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <param name="password">password to visit a shortlink (only needed if a shortlink was set up with a password)</param>
  /// <returns>Metadata of the shortlink, 404 if the shortlink does not (yet) exist</returns>
  [HttpGet("/links/{shortLinkId}/meta", Name = "GetMetaData")]
  [ProducesResponseType(typeof(ShortLinkMetaData), 200, "application/json")]
  [ProducesResponseType((int)HttpStatusCode.NotFound)]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  public async Task<ActionResult> Get(string shortLinkId, [FromQuery] string? password) {
    var shortLinkResult = await shortLinkRepository.GetAsync(shortLinkId);
    if (shortLinkResult.IsFailure)
      return Problem();
    if (shortLinkResult.Value.HasNoValue)
      return NotFound();

    if (shortLinkResult.Value.Value.PasswordHash != null
        && !PasswordHasher.VerifyPasswordHash(shortLinkResult.Value.Value.PasswordHash, password, shortLinkId))
      return Unauthorized();

    var response = await metaDataRepository.GetAsync(shortLinkId)
      .TapError(e => logger.LogError("Could not get MetaData for {Id}, because {Error}", shortLinkId, e))
      .Tap(e => e.Execute(m => metricsRegistry.Success(m.Id)))
      .Match(maybeMetaData => maybeMetaData.HasValue ? (ActionResult)Ok(maybeMetaData.Value) : NotFound(), BadRequest);

    return response;
  }
}
