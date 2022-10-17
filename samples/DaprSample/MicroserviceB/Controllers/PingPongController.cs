using CodeArchitects.Platform.Infrastructure.State;
using MicroserviceB.Domain.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceB.Controllers;

[ApiController]
[Route("pingpong")]
public class PingPongController : ControllerBase
{
  private readonly IStateStore _store;

  public PingPongController(IStateStore store)
  {
    _store = store;
  }

  [HttpGet("pong")]
  public async Task<IActionResult> Ping()
  {
    PongMessage? message = await _store.GetAsync<PongMessage>("pong");
    if (message is null)
      return NotFound();

    return Ok(message);
  }
}
