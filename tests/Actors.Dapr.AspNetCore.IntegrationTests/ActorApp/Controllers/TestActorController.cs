using ActorApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ActorApp.Controllers;

[ApiController]
[Route("test-actor")]
[ErrorFilter]
public class TestActorController : ControllerBase
{
  private readonly ITestActorFactory _actorFactory;

  public TestActorController(ITestActorFactory actorFactory)
  {
    _actorFactory = actorFactory;
  }

  [HttpGet("create")]
  public async Task<ActionResult> CreateAsync(int state)
  {
    string id = Guid.NewGuid().ToString();
    _ = await _actorFactory.CreateAsync(id, state);

    return Ok(new
    {
      Id = id
    });
  }

  [HttpGet("{id}/state")]
  public async Task<ActionResult> GetStateAsync(string id)
  {
    ITestActor actor = _actorFactory.Get(id);

    return Ok(new
    {
      State = await actor.GetStateAsync()
    });
  }
}
