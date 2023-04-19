using ActorApp.Domain;
using CodeArchitects.Platform.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ActorApp.Controllers;

[ApiController]
[Route("virtual-actor")]
public class VirtualActorController : ControllerBase
{
  private readonly IVirtualActorFactory _actorFactory;
  private readonly IMessageBus _bus;
  private readonly ActorOutput _output;

  public VirtualActorController(IVirtualActorFactory actorFactory, IMessageBus bus, ActorOutput output)
  {
    _actorFactory = actorFactory;
    _bus = bus;
    _output = output;
  }

  [HttpGet("{id}/output")]
  public ActionResult Output(Guid id)
  {
    return Ok(new
    {
      Output = _output.GetOutput(id)
    });
  }

  [HttpGet("polymorphic-method")]
  public async Task<ActionResult> PolymorphicMethod(int implementation)
  {
    Guid id = Guid.NewGuid();
    IVirtualActor actor = _actorFactory.Get(id);

    if (implementation != 0)
    {
      await actor.BecomeAsync(implementation);
    }
    int result = await actor.PolymorphicMethodAsync();

    return Ok(new
    {
      Implementation = result
    });
  }

  [HttpGet("schedule")]
  public async Task<ActionResult> Schedule(string output)
  {
    Guid id = Guid.NewGuid();
    IVirtualActor actor = _actorFactory.Get(id);

    await actor.ScheduleAsync(output);

    return Ok(new
    {
      Id = id
    });
  }

  [HttpGet("binding-enabler")]
  public async Task<ActionResult> BindingEnabler()
  {
    Guid id = Guid.NewGuid();
    IVirtualActor actor = _actorFactory.Get(id);

    await actor.BindingEnablerAsync();

    return Ok(new
    {
      Output = _output.GetOutput(id)
    });
  }

  [HttpGet("binding-disabler")]
  public async Task<ActionResult> BindingDisabler()
  {
    Guid id = Guid.NewGuid();
    IVirtualActor actor = _actorFactory.Get(id);

    await actor.BindingDisablerAsync();

    return Ok(new
    {
      Output = _output.GetOutput(id) ?? "no binding"
    });
  }

  [HttpGet("messaging")]
  public async Task<ActionResult> Messaging(string output)
  {
    Guid id = Guid.NewGuid();
    await _bus.SendAsync(new TestMessage { ActorId = id, Output = output });

    return Ok(new
    {
      Id = id
    });
  }
}
