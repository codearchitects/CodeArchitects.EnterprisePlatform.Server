using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class ManagerFactory<TActor, TState> : IManagerFactory<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  private readonly IServiceProvider _services;
  private readonly IActorDescriptor<TActor, TState> _descriptor;
  private readonly IActivityManager<TActor> _activityManager;

  public ManagerFactory(IServiceProvider services, IActorModel model, IActivityManager<TActor> activityManager)
  {
    _services = services;
    _descriptor = model.GetActor<TActor, TState>();
    _activityManager = activityManager;
  }

  public IActorManager<TActor, TState> Create(IActorHost<TActor> host, TState? state)
  {
    state ??= GetDefaultState(host.ActorId);
    return CreateCore(host, state, state.ImplementationId);
  }

  public IActorManager<TActor, TState> Create(IActorHost<TActor> host, TState? state, int implementationId)
  {
    state ??= GetDefaultState(host.ActorId);
    if (implementationId == 0)
    {
      implementationId = state.ImplementationId;
    }

    return CreateCore(host, state, implementationId);
  }

  public void InitializeState(TState state)
  {
    if (_descriptor.IsPolymorphic)
    {
      state.ImplementationId = _descriptor.DefaultImplementation.Id;
    }
  }

  private IActorManager<TActor, TState> CreateCore(IActorHost<TActor> host, TState state, int implementationId)
  {
    return new ActorContext<TActor, TState>(_services, _descriptor, _activityManager, host, state, implementationId);
  }

  private TState GetDefaultState(string id)
  {
    if (!_descriptor.IsVirtual)
      throw new UninitializedActorException(typeof(TActor));

    TState defaultValue = _descriptor.State.DefaultValue!;
    defaultValue.ImplementationId = _descriptor.DefaultImplementation.Id;
    _descriptor.Id.SetId(defaultValue, id);

    return defaultValue;
  }
}
