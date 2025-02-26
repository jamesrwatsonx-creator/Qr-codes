using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Flink.MetaData.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class MetaDataController(IMetaDataRepository metaDataRepository, ILogger<MetaDataController> logger) : ControllerBase {
  [HttpGet("/meta/{id}")]
  [ProducesResponseType(typeof(ShortLinkMetaData), 200, "application/json")]
  [ProducesResponseType(typeof(ActionResult), 404, "application/json")]
  [ProducesResponseType(typeof(ActionResult), 500, "application/json")]
  public Task<ActionResult> Get(string id) =>
    metaDataRepository.GetAsync(id)
      .TapError(e => logger.LogError("Could not get MetaData for {Id}, because {Error}", id, e))
      .Match(maybeMetaData => maybeMetaData.HasValue ? (ActionResult)Ok(maybeMetaData.Value) : NotFound(), BadRequest);
}
