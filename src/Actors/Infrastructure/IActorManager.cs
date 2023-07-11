using CodeArchitects.Platform.Actors.Scheduling;
using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorManager<TActor>
  where TActor : class
{
  TActor Actor { get; }
  
  int DefaultImplementationId { get; }
  
  JsonSerializerOptions JsonSerializerOptions { get; }
  
  Type ActivityType { get; }

  void InitializeState(ActorState state);

  Task BeginMethodAsync(CancellationToken cancellationToken);

  Task BeginActivityAsync(Activity<TActor> activity, CancellationToken cancellationToken);

  Task EndExecutionAsync(CancellationToken cancellationToken);
}
