using CodeArchitects.Platform.Infrastructure.State;
using CodeArchitects.Platform.Messaging;
using MicroserviceA.Domain.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceA.Controllers;

[ApiController]
[Route("pingpong")]
public class PingPongController : ControllerBase
{
  private readonly IMessageBus _bus;
  private readonly IStateStore _store;

  public PingPongController(IMessageBus bus, IStateStore store)
  {
    _bus = bus;
    _store = store;
  }

  [HttpPost]
  public async Task<IActionResult> PingPong()
  {
    await _bus.SendAsync("ping", new PingMessage(Guid.NewGuid(), 0));

    return Ok();
  }

  [HttpGet("ping")]
  public async Task<IActionResult> Ping()
  {
    PingMessage? message = await _store.GetAsync<PingMessage>("ping");
    return message is null ? NotFound() : Ok(message);
  }
}
