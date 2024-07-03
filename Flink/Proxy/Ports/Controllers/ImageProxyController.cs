using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Flink.Proxy.Ports.Controllers;

[ApiController]
public class ImageProxyController(HttpClient httpClient) : ControllerBase {
  [HttpGet("/proxy")]
  [ProducesResponseType(typeof(string), 200, "text/html")]
  public async Task<IActionResult> ProxyImage([FromQuery] string url) {
    if (string.IsNullOrWhiteSpace(url))
      return BadRequest("URL parameter is required.");
    
    if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
      return BadRequest("Invalid URL.");


    try {
      var response = await httpClient.GetAsync(uriResult);
      var content = await response.Content.ReadAsByteArrayAsync();

      // Return the response content as the result
      return File(content, response.Content.Headers.ContentType?.ToString()!);
    } catch (HttpRequestException) {
      // Handle the error appropriately
      return StatusCode((int)HttpStatusCode.BadRequest);
    }
  }
}
