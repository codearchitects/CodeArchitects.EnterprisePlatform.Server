using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorManager<TActor, TState>
  where TActor : class
{
  TActor Actor { get; }
  
  TState State { get; }
  
  int DefaultImplementationId { get; }
  
  JsonSerializerOptions JsonSerializerOptions { get; }
  
  Type ActivityType { get; }
  
  TState DefaultState { get; }

  void OnActivityBegin();

  void OnMethodBegin();

  Task OnExecutionEndAsync(CancellationToken cancellationToken);
}
