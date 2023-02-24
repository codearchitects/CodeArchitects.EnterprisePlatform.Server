using ActorApp.Domain;
using CodeArchitects.Platform.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ActorApp.Controllers;

[ApiController]
[Route("intersection")]
public class IntersectionController : ControllerBase
{
  private readonly ITrafficLightFactory _trafficLightFactory;
  private readonly IMessageBus _bus;

  public IntersectionController(ITrafficLightFactory cartActorFactory, IMessageBus bus)
  {
    _trafficLightFactory = cartActorFactory;
    _bus = bus;
  }

  [HttpGet("startnew")]
  public async Task<ActionResult> StartAsync()
  {
    Guid id = Guid.NewGuid();
    ITrafficLight trafficLight = _trafficLightFactory.Get(id);

    await trafficLight.TurnOnAsync();

    return Ok(new
    {
      Message = $"Traffic light {id} started"
    });
  }

  [HttpGet("{id}/stop")]
  public async Task<ActionResult> StopAsync(Guid id)
  {
    await _bus.SendAsync("traffic-light", new TurnOffCommand { ActorId = id, Reason = "user request" });

    return Ok(new
    {
      Message = $"Traffic light {id} stopped"
    });
  }

  [HttpGet("{id}/color")]
  public async Task<ActionResult> GetColorAsync(Guid id)
  {
    ITrafficLight trafficLight = _trafficLightFactory.Get(id);

    string color = await trafficLight.GetLightColorAsync();

    return Ok(new
    {
      Message = $"The light is currently {color}"
    });
  }

  [HttpGet("{id}/cross")]
  public async Task<ActionResult> CrossAsync(Guid id)
  {
    ITrafficLight trafficLight = _trafficLightFactory.Get(id);

    var response = await trafficLight.CrossIntersectionAsync();

    string message = response.CanCross
      ? "Go ahead!"
      : $"Stop! Light turns green in {response.TurnsGreenAt - DateTime.Now}";

    return Ok(new
    {
      Message = message
    });
  }
}
