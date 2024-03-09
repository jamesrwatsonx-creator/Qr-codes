using Flink.Index.Ports.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flink.Index.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class IndexController : ControllerBase {
  [HttpGet("/")]
  [ProducesResponseType(typeof(IndexResponse), 200, "application/json")]
  public IndexResponse Get() {
    return new IndexResponse();
  }
}
