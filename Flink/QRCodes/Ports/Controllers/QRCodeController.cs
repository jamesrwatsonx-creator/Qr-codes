using CSharpFunctionalExtensions;
using Flink.ShortLinks.Ports;
using Microsoft.AspNetCore.Mvc;


namespace Flink.QRCodes.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class QRCodeController(IShortLinkRepository shortLinkRepository, ILogger<QRCodeController> logger) : ControllerBase {
  [HttpGet("/qr/{id}")]
  [ProducesResponseType(typeof(byte[]), 200, "image/png")]
  [ProducesResponseType(typeof(ActionResult), 404, "application/json")]
  [ProducesResponseType(typeof(ActionResult), 500, "application/json")]
  public Task<ActionResult> Get(string id) {
    logger.LogInformation("Requesting Short Link with {Id}", id);
    
    return shortLinkRepository
      .GetAsync(id)
      .Match(maybeLink => maybeLink
        .Match(l => (ActionResult)File(QrCodeGenerator.CreateUriQrCode(l.Uri), "image/png"), NotFound), _ => Problem());
  }
}
