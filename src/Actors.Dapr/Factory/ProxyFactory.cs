using Dapr.Actors;
using Dapr.Actors.Client;

namespace CodeArchitects.Platform.Actors.Dapr.Factory;

internal abstract class ProxyFactory<THostInterface, TActorId, TInterface, TState> : ActorHostFactory<THostInterface, TActorId>
  where THostInterface : IActor
  where TActorId : notnull
{
  protected ProxyFactory(IActorProxyFactory hostFactory)
    : base(hostFactory)
  {
  }

  protected abstract TInterface CreateProxy(THostInterface actorHost);

  protected abstract Task InitAsync(THostInterface actorHost, TState state, CancellationToken cancellationToken);

  protected Task<TInterface> CreateAsync(TActorId actorId, TState state, CancellationToken cancellationToken)
  {
    if (actorId is null)
      throw new ArgumentNullException(nameof(actorId));
    
    return CreateCoreAsync(actorId, state, cancellationToken);
  }

  protected TInterface GetCore(TActorId actorId)
  {
    if (actorId is null)
      throw new ArgumentNullException(nameof(actorId));

    THostInterface host = CreateHost(actorId);
    return CreateProxy(host);
  }

  private async Task<TInterface> CreateCoreAsync(TActorId actorId, TState state, CancellationToken cancellationToken)
  {
    THostInterface host = CreateHost(actorId);
    await InitAsync(host, state, cancellationToken);
    return CreateProxy(host);
  }
}
