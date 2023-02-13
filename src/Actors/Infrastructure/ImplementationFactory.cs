using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal abstract class ImplementationFactory<TActor, TState> : IImplementationFactory<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  private readonly IActorManager<TActor, TState> _manager;
  private readonly IActivityManager _activityManager;

  protected ImplementationFactory(IActorManager<TActor, TState> manager, IActivityManager activityManager)
  {
    _manager = manager;
    _activityManager = activityManager;
  }

  public abstract TActor Create(IActorHost<TActor, TState> host, TState state, int implementationId);
  
  public TActor Create(IActorHost<TActor, TState> host, TState state)
  {
    return Create(host, state, state.ImplementationId);
  }

  protected ActorContext<TActor, TState> CreateContext(IActorHost<TActor, TState> host, int implementationId)
  {
    return new(_manager, _activityManager, host, implementationId);
  }
}
