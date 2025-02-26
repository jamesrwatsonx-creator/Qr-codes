using Flink.Info.Ports.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flink.Info.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class InfoController : ControllerBase {
  [HttpGet("/info")]
  [ProducesResponseType(typeof(IndexResponse), 200, "application/json")]
  public IndexResponse Get() {
    return new IndexResponse();
  }
}
