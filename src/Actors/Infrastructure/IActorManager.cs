using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorManager<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  TState DefaultState { get; }

  int DefaultImplementationId { get; }
  
  Type ActivityType { get; }

  JsonSerializerOptions JsonSerializerOptions { get; }

  int GetImplementationId(Type implementationType);

  void UpdateState(TActor actor, TState state);
}
