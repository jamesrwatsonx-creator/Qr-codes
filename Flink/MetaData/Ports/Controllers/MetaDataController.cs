using System.Net;
using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Flink.MetaData.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class MetaDataController(IMetaDataRepository metaDataRepository, ILogger<MetaDataController> logger) : ControllerBase {
  /// <summary>
  ///   Query the crawled/scraped metadata for any given shortlink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <returns>Metadata of the shortlink, 404 if the shortlink does not (yet) exist</returns>
  [HttpGet("/links/{shortLinkId}/meta", Name = "GetMetaData")]
  [ProducesResponseType(typeof(ShortLinkMetaData), 200, "application/json")]
  [ProducesResponseType((int)HttpStatusCode.NotFound)]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  public Task<ActionResult> Get(string shortLinkId) {
    return metaDataRepository.GetAsync(shortLinkId)
      .TapError(e => logger.LogError("Could not get MetaData for {Id}, because {Error}", shortLinkId, e))
      .Match(maybeMetaData => maybeMetaData.HasValue ? (ActionResult)Ok(maybeMetaData.Value) : NotFound(), BadRequest);
  }
}
