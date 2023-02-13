using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class ManagerFactory<TActor, TState> : IManagerFactory<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  private readonly IServiceProvider _services;
  private readonly IActorDescriptor<TActor, TState> _descriptor;
  private readonly IActivityManager _activityManager;

  public ManagerFactory(IServiceProvider services, IActorModel model, IActivityManager activityManager)
  {
    _services = services;
    _descriptor = model.GetDescriptor<TActor, TState>();
    _activityManager = activityManager;
  }

  public IActorManager<TActor, TState> Create(IActorHost<TActor, TState> host, TState state)
  {
    return Create(host, state, state.ImplementationId);
  }

  public IActorManager<TActor, TState> Create(IActorHost<TActor, TState> host, TState state, int implementationId)
  {
    ActorContext<TActor, TState> context = new(state, _descriptor, _activityManager, host, implementationId);
    context.Actor = _descriptor.CreateInstance(implementationId, _services, state, context);

    return context;
  }
}
