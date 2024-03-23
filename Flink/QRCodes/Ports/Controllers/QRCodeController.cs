using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.QRCodes.Repositories;
using Flink.Shared.Extensions;
using Flink.ShortLinks.Ports;
using Microsoft.AspNetCore.Mvc;


namespace Flink.QRCodes.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class QrCodeController(IShortLinkRepository shortLinkRepository, IQrCodeCache qrCodeCache, ILogger<QrCodeController> logger) : ControllerBase {
  [HttpGet("/qr/{id}")]
  [ProducesResponseType(typeof(byte[]), 200, "image/png")]
  [ProducesResponseType(typeof(ActionResult), 404, "application/json")]
  [ProducesResponseType(typeof(ActionResult), 500, "application/json")]
  public Task<ActionResult> Get(string id, [FromQuery] int height = 512) {
    logger.LogInformation("Requesting Short Link with {Id}", id);
    return shortLinkRepository
      .GetAsync(id)
      .MapError(_ => (ActionResult)Problem())
      .Bind(maybeShortenedLink => maybeShortenedLink.ToResult((ActionResult)NotFound()))
      .Map(shortenedLink => shortenedLink.Uri)
      .Match(uri => qrCodeCache.Retrieve(uri, height)
          .Map(maybeQrCode => maybeQrCode.GetValueOrDefault(QrCodeGenerator.CreateUriQrCode(uri, height)))
          .Bind(qrCode => qrCodeCache.Cache(id, height, qrCode))
          .Match(qrCode => (ActionResult)File(qrCode, "image/png"), _ => Problem())
        , e => e);
  }
}
