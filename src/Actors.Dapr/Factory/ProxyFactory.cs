using Dapr.Actors.Client;
using Dapr.Actors;

namespace CodeArchitects.Platform.Actors.Dapr.Factory;

internal abstract class ProxyFactory<THostInterface, TInterface, TState>
  where THostInterface : IActor
{
  private readonly IActorProxyFactory _actorFactory;

  protected ProxyFactory(IActorProxyFactory actorFactory)
  {
    _actorFactory = actorFactory;
  }

  protected abstract string ActorName { get; }

  protected abstract TInterface CreateProxy(THostInterface actorHost);

  protected abstract Task InitAsync(THostInterface actorHost, TState state, CancellationToken cancellationToken);

  protected async Task<TInterface> CreateCoreAsync<TActorId>(TActorId actorId, TState state, CancellationToken cancellationToken)
    where TActorId : notnull
  {
    if (actorId is null)
      throw new ArgumentNullException(nameof(actorId));

    THostInterface actorHost = _actorFactory.CreateActorProxy<THostInterface>(new ActorId(actorId.ToString()), ActorName);
    await InitAsync(actorHost, state, cancellationToken);
    return CreateProxy(actorHost);
  }

  protected TInterface GetCore<TActorId>(TActorId actorId)
    where TActorId : notnull
  {
    if (actorId is null)
      throw new ArgumentNullException(nameof(actorId));

    THostInterface actor = _actorFactory.CreateActorProxy<THostInterface>(new ActorId(actorId.ToString()), ActorName);
    return CreateProxy(actor);
  }
}
