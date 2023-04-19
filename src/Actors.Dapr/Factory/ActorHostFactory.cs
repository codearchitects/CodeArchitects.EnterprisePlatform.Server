using Dapr.Actors;
using Dapr.Actors.Client;

namespace CodeArchitects.Platform.Actors.Dapr.Factory;

internal abstract class ActorHostFactory<THostInterface, TActorId>
  where THostInterface : IActor
  where TActorId : notnull
{
  private readonly IActorProxyFactory _hostFactory;

  protected ActorHostFactory(IActorProxyFactory hostFactory)
  {
    _hostFactory = hostFactory;
  }

  protected abstract string ActorName { get; }

  protected THostInterface CreateHost(TActorId id)
  {
    return _hostFactory.CreateActorProxy<THostInterface>(new ActorId(id.ToString()), ActorName);
  }
}
