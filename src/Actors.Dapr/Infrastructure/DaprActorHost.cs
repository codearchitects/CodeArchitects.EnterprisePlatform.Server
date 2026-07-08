using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
using Dapr.Actors.Runtime;
using System.Diagnostics;
using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

internal class DaprActorHost<TActor, TState> : Actor, IActorHost<TActor, TState>, IRemindable
  where TActor : class
  where TState : ActorState
{
  private readonly IActorManager<TActor> _manager;

  protected DaprActorHost(ActorHost host, IActorManagerFactory<TActor, TState> managerFactory)
    : base(host)
  {
    _manager = managerFactory.CreateManager(this);
  }

  public string ActorId => Id.GetId();

  protected TActor Actor => _manager.Actor;

  protected override async Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
  {
    if (!IsInterfaceMethod(actorMethodContext))
      return;

    await _manager.BeginMethodAsync(CancellationToken.None);
  }

  protected override async Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
  {
    if (!IsInterfaceMethod(actorMethodContext))
      return;

    await _manager.EndMethodAsync(CancellationToken.None);
  }

  protected Task InitAsync(byte[] payload, CancellationToken cancellationToken)
  {
    TState state = JsonSerializer.Deserialize<TState>(payload)!;
    _manager.InitializeState(state);
    return StateManager.AddStateAsync(Constants.ActorStateName, state, cancellationToken);
  }

  public Task ScheduleAsync(ScheduleId id, Activity<TActor> activity, SchedulingOptions options, CancellationToken cancellationToken)
  {
    byte[] payload = JsonSerializer.SerializeToUtf8Bytes(activity, _manager.ActivityType, _manager.JsonSerializerOptions);

    return RegisterReminderAsync(id.Id, payload, options.Timer, options.Period);
  }

  public Task UnscheduleAsync(ScheduleId id, CancellationToken cancellationToken)
  {
    return UnregisterReminderAsync(id.Id);
  }

  public async Task ReceiveReminderAsync(string reminderName, byte[] payload, TimeSpan dueTime, TimeSpan period)
  {
    Activity<TActor>? activity = (Activity<TActor>?)JsonSerializer.Deserialize(payload, _manager.ActivityType, _manager.JsonSerializerOptions);
    Debug.Assert(activity is not null, "Deserializing activity payload returned null.");

    await _manager.BeginActivityAsync(activity, CancellationToken.None);
    await activity.ExecuteAsync(_manager.Actor, CancellationToken.None);
    await _manager.EndActivityAsync(CancellationToken.None);
  }

  public async Task<TState?> GetStateAsync(CancellationToken cancellationToken)
  {
    ConditionalValue<TState> state = await StateManager.TryGetStateAsync<TState>(Constants.ActorStateName, cancellationToken);

    if (state.HasValue)
      return state.Value;

    return null;
  }

  public async Task SetStateAsync(TState state, CancellationToken cancellationToken)
  {
    await StateManager.SetStateAsync(Constants.ActorStateName, state, CancellationToken.None);
  }

  private static bool IsInterfaceMethod(ActorMethodContext actorMethodContext)
  {
    return
      actorMethodContext.CallType == ActorCallType.ActorInterfaceMethod &&
      actorMethodContext.MethodName != Constants.InitAsyncMethodName;
  }
}
