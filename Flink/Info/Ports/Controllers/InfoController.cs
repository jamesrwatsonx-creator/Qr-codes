using Flink.Info.Ports.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flink.Info.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class InfoController : ControllerBase {
  /// <summary>
  ///   Return information about this flink instance
  /// </summary>
  /// <returns></returns>
  [HttpGet("/info", Name = "GetInstanceInfo")]
  [ProducesResponseType(typeof(IndexResponse), 200, "application/json")]
  public IndexResponse Get() {
    return new IndexResponse();
  }
}
