using System.Net;
using CSharpFunctionalExtensions;
using Flink.QRCodes.Repositories;
using Flink.ShortLinks.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Flink.QRCodes.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class QrCodeController(
  IShortLinkRepository shortLinkRepository,
  IQrCodeCache qrCodeCache,
  QrCodeMetricsRegistry metricsRegistry,
  ILogger<QrCodeController> logger) : ControllerBase {
  /// <summary>
  ///   Get the QR Code for a (previously created) shortlink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <param name="height">the height of the QR Code in px</param>
  /// <returns>QR Code as Portable Network Graphic (*.png) if the shortlink exists - else 404</returns>
  [HttpGet("/links/{shortLinkId}/qr", Name = "GetQRCode")]
  [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK, "image/png")]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  [ProducesResponseType((int)HttpStatusCode.NotFound)]
  public Task<ActionResult> Get(string shortLinkId, [FromQuery] int height = 512) {
    if (height <= 32)
      return Task.FromResult<ActionResult>(BadRequest());

    logger.LogInformation("Requesting Short Link with {Id}", shortLinkId);
    return shortLinkRepository
      .GetAsync(shortLinkId)
      .MapError(ActionResult (_) => Problem())
      .Bind(maybeShortenedLink => maybeShortenedLink.ToResult((ActionResult)NotFound()))
      .Map(shortenedLink => shortenedLink.Uri)
      .Match(uri => qrCodeCache.Retrieve(uri, height)
          .Map(maybeQrCode => maybeQrCode.GetValueOrDefault(QrCodeGenerator.CreateUriQrCode(uri, height)))
          .Bind(qrCode => qrCodeCache.Cache(shortLinkId, height, qrCode))
          .Tap(_ => metricsRegistry.Success(shortLinkId))
          .Match(ActionResult (qrCode) => File(qrCode, "image/png"), _ => Problem())
        , e => e);
  }
}
