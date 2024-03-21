using Microsoft.AspNetCore.Mvc;
using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.Shared.Extensions;
using Flink.ShortLinks.Models;

namespace Flink.ShortLinks.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class ShortLinkController(IShortLinkRepository shortLinkRepository, ILogger<ShortLinkController> logger, ShortLinkMetricsRegistry metrics) : ControllerBase {
  [HttpGet("/to/{id}")]
  [ProducesResponseType(typeof(RedirectResult), 301, "application/json")]
  [ProducesResponseType(typeof(ActionResult), 404, "application/json")]
  [ProducesResponseType(typeof(ActionResult), 500, "application/json")]
  public Task<ActionResult> Get(string id) {
    logger.LogDebug("Requesting Short Link with {Id}", id);
    
    return shortLinkRepository
      .GetAsync(id)
      .TapError(e => logger.LogError("Could not read Shortened Link with {Id}, because {Error}", id, e))
      .Match(maybeLink => maybeLink
        .TapSome(_ => metrics.Ok(id))
        .TapNone(() => metrics.NotFound(id))
        .Match(l => (ActionResult)Redirect(l.Uri), NotFound), _ => Problem());
  }

  [HttpPost("/to")]
  [ProducesResponseType(typeof(ActionResult<ShortenedLink>), 201, "application/json")]
  [ProducesResponseType(typeof(ActionResult), 400, "application/json")]
  [ProducesResponseType(typeof(ActionResult), 500, "application/json")]
  public Task<ActionResult> Add(ShortLinkRequest shortLinkRequest) {
    var shortenedLink = new ShortenedLink {
      Id = shortLinkRequest.PreferredId ?? ShortLinkIdGenerator.CreateRandomId(shortLinkRequest.PreferredIdLength),
      Creator = shortLinkRequest.Creator,
      Uri = shortLinkRequest.Uri,
      CreationDate = DateTime.UtcNow
    };

    return shortLinkRepository
      .AddAsync(shortenedLink)
      .Tap(shortLink => logger.LogInformation("Saved Shortened Link with {Id}", shortLink.Id))
      .Match(shortLink => (ActionResult)Created($"/to/{shortLink.Id}", shortLink), BadRequest);
  }
}
