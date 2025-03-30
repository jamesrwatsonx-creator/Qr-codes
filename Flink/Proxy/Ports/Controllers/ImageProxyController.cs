using System.Net;
using CSharpFunctionalExtensions;
using Flink.MetaData.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Flink.Proxy.Ports.Controllers;

[ApiController]
public class ImageProxyController(IMetaDataRepository metaDataRepository, IForwardProxy forwardProxy) : ControllerBase {
  /// <summary>
  ///   Get a proxied and cached preview image/thumbnail for a shortlink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <returns>The preview image with its original content-type headers, a QR code if no image was identified, or 404 if the shortlink is unknown</returns>
  [HttpGet("/links/{shortLinkId}/thumbnail", Name = "GetThumbnail")]
  [ProducesResponseType(typeof(byte[]), 200, "image/png", "image/jpeg", "image/bmp", "image/tiff", "image/gif", "image/webp")]
  [ProducesResponseType((int)HttpStatusCode.Redirect)]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  public async Task<IActionResult> ProxyImage(string shortLinkId) {
    if (string.IsNullOrWhiteSpace(shortLinkId))
      return BadRequest("URL parameter is required.");

    var r = await metaDataRepository.GetAsync(shortLinkId);
    if (r.IsFailure)
      return BadRequest($"Failed to read ShortLink MetaData for entry with Id '{shortLinkId}'");
    if (r is { IsSuccess: true, Value.HasNoValue: true })
      return NotFound($"The given ShortLink MetaData fo the Id '{shortLinkId}' does not (yet) exist");
    if (r is { IsSuccess: true, Value: { HasValue: true, Value.Image: null } })
      return Redirect($"/links/{shortLinkId}/qr?height=512");

    return await forwardProxy.Proxy(r.Value.Value.Image!)
      .Match(fileContentResult => fileContentResult, IActionResult (f) => BadRequest(f));
  }
}
