using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorManager<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  TActor Actor { get; }
  
  TState State { get; }
  
  int DefaultImplementationId { get; }
  
  JsonSerializerOptions JsonSerializerOptions { get; }
  
  Type ActivityType { get; }

  void OnActivityBegin();

  void OnMethodBegin();

  void OnExecutionEnd();

  Task ExecuteBindingsAsync(CancellationToken cancellationToken);
}
