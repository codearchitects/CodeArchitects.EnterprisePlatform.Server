using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[Actor]
public class TestActor : ITestActor
{
  [State] private readonly int _state;

  public TestActor(int state)
  {
    _state = state;
  }

  public Task<int> GetStateAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromResult(_state);
  }
}
