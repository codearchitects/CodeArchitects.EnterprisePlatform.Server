using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class ActorManagerFactory<TActor, TState> : IActorManagerFactory<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  private readonly IServiceProvider _services;
  private readonly IActorDescriptor<TActor, TState> _descriptor;
  private readonly IActivityManager<TActor> _activityManager;

  public ActorManagerFactory(IServiceProvider services, IActorDescriptor<TActor, TState> descriptor, IActivityManager<TActor> activityManager)
  {
    _services = services;
    _descriptor = descriptor;
    _activityManager = activityManager;
  }

  public IActorManager<TActor> CreateManager(IActorHost<TActor, TState> host)
  {
    return new ActorContext<TActor, TState>(_services, _descriptor, _activityManager, host);
  }
}
