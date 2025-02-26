using CSharpFunctionalExtensions;
using Flink.MetaData.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Flink.Proxy.Ports.Controllers;

[ApiController]
public class ImageProxyController(IMetaDataRepository metaDataRepository, IForwardProxy forwardProxy) : ControllerBase {
  [HttpGet("/proxy/image/{shortLinkId}")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public async Task<IActionResult> ProxyImage(string shortLinkId) {
    if (string.IsNullOrWhiteSpace(shortLinkId))
      return BadRequest("URL parameter is required.");

    var r = await metaDataRepository.GetAsync(shortLinkId);
    if (r.IsFailure)
      return BadRequest($"Failed to read ShortLink MetaData for entry with Id '{shortLinkId}'");
    if (r is { IsSuccess: true, Value.HasNoValue: true })
      return NotFound($"The given ShortLink MetaData fo the Id '{shortLinkId}' does not (yet) exist");
    if (r is { IsSuccess: true, Value.HasValue: true } && r.Value.Value.Image == null)
      return Redirect($"/qr/{shortLinkId}?height=512");

    return await forwardProxy.Proxy(r.Value.Value.Image!)
      .Match(fileContentResult => fileContentResult, f => (IActionResult)BadRequest(f));
  }
}
