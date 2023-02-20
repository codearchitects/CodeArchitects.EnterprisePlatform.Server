using ActorApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ActorApp.Controllers;

[ApiController]
[Route("actor")]
public class ActorController : ControllerBase
{
  private readonly ITestActorFactory _actorFactory;
  private readonly ActorOutput _output;

  public ActorController(ITestActorFactory actorFactory, ActorOutput output)
  {
    _actorFactory = actorFactory;
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
    ITestActor actor = _actorFactory.Get(id);

    await actor.BecomeAsync(implementation);
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
    ITestActor actor = _actorFactory.Get(id);

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
    ITestActor actor = _actorFactory.Get(id);

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
    ITestActor actor = _actorFactory.Get(id);

    await actor.BindingDisablerAsync();

    return Ok(new
    {
      Output = _output.GetOutput(id) ?? "no binding"
    });
  }
}
