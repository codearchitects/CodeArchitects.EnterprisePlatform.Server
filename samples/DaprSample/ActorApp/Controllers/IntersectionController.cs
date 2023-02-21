using ActorApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ActorApp.Controllers;

[ApiController]
[Route("intersection")]
public class IntersectionController : ControllerBase
{
  private readonly ITrafficLightFactory _trafficLightFactory;

  public IntersectionController(ITrafficLightFactory cartActorFactory)
  {
    _trafficLightFactory = cartActorFactory;
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
    ITrafficLight trafficLight = _trafficLightFactory.Get(id);

    await trafficLight.TurnOffAsync();

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
