using ActorApp.OldSkool;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;

namespace ActorApp.Controllers;

[ApiController]
[Route("intersection-oldskool")]
public class OldSkoolIntersectionController : ControllerBase
{
  private readonly IActorProxyFactory _actorFactory;

  public OldSkoolIntersectionController(IActorProxyFactory actorFactory)
  {
    _actorFactory = actorFactory;
  }

  [HttpGet("startnew")]
  public async Task<ActionResult> StartAsync()
  {
    Guid id = Guid.NewGuid();
    ITrafficLightActor actor = _actorFactory.CreateActorProxy<ITrafficLightActor>(new ActorId(id.ToString()), nameof(TrafficLightActor));

    await actor.TurnOnAsync();

    return Ok(new
    {
      Message = $"Traffic light {id} started"
    });
  }

  [HttpGet("{id}/stop")]
  public async Task<ActionResult> StopAsync(Guid id)
  {
    ITrafficLightActor actor = _actorFactory.CreateActorProxy<ITrafficLightActor>(new ActorId(id.ToString()), nameof(TrafficLightActor));

    await actor.TurnOffAsync();

    return Ok(new
    {
      Message = $"Traffic light {id} stopped"
    });
  }

  [HttpGet("{id}/color")]
  public async Task<ActionResult> GetColorAsync(Guid id)
  {
    ITrafficLightActor actor = _actorFactory.CreateActorProxy<ITrafficLightActor>(new ActorId(id.ToString()), nameof(TrafficLightActor));

    string color = await actor.GetLightColorAsync();

    return Ok(new
    {
      Message = $"The light is currently {color}"
    });
  }

  [HttpGet("{id}/cross")]
  public async Task<ActionResult> CrossAsync(Guid id)
  {
    ITrafficLightActor actor = _actorFactory.CreateActorProxy<ITrafficLightActor>(new ActorId(id.ToString()), nameof(TrafficLightActor));

    var response = await actor.CrossIntersectionAsync();

    string message = response.CanCross
      ? "Go ahead!"
      : $"Stop! Light turns green in {response.TurnsGreenAt - DateTime.Now}";

    return Ok(new
    {
      Message = message
    });
  }
}
