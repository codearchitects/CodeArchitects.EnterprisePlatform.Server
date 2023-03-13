using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
using Dapr.Actors.Runtime;
using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

internal class DaprActorHost<TActor, TState> : Actor, IActorHost<TActor>, IRemindable
  where TActor : class
  where TState : ActorState
{
  private readonly IManagerFactory<TActor, TState> _factory;

  protected DaprActorHost(ActorHost host, IManagerFactory<TActor, TState> factory)
    : base(host)
  {
    _factory = factory;
  }

  public IActorManager<TActor, TState> Manager { get; set; } = default!;

  public string ActorId => Id.GetId();

  protected TActor Actor => Manager.Actor;

  protected override async Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
  {
    if (actorMethodContext.CallType is not ActorCallType.ActorInterfaceMethod)
      return;
    if (actorMethodContext.MethodName is Constants.InitAsyncMethodName)
      return;

    TState? state = await GetStateAsync(CancellationToken.None);
    Manager = _factory.Create(this, state);
    Manager.OnMethodBegin();
  }

  protected override async Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
  {
    if (actorMethodContext.MethodName is Constants.InitAsyncMethodName)
      return;

    await Manager.ExecuteBindingsAsync(CancellationToken.None);
    Manager.OnExecutionEnd();
    await StateManager.SetStateAsync(Constants.ActorStateName, Manager.State, CancellationToken.None);
  }

  protected Task InitAsync(byte[] payload, CancellationToken cancellationToken)
  {
    TState state = JsonSerializer.Deserialize<TState>(payload)!;
    _factory.InitializeState(state);
    return StateManager.AddStateAsync(Constants.ActorStateName, state, cancellationToken);
  }

  public Task ScheduleAsync(ScheduleId id, Activity<TActor> activity, SchedulingOptions options, CancellationToken cancellationToken)
  {
    byte[] payload = JsonSerializer.SerializeToUtf8Bytes(activity, Manager.ActivityType, Manager.JsonSerializerOptions);

    return RegisterReminderAsync(id, payload, options.Timer, options.Period);
  }

  public Task UnscheduleAsync(ScheduleId id, CancellationToken cancellationToken)
  {
    return UnregisterReminderAsync(id);
  }

  public async Task ReceiveReminderAsync(string reminderName, byte[] payload, TimeSpan dueTime, TimeSpan period)
  {
    Activity<TActor> activity = (Activity<TActor>)JsonSerializer.Deserialize(payload, Manager.ActivityType, Manager.JsonSerializerOptions)!;

    TState? state = await GetStateAsync(CancellationToken.None);
    Manager = _factory.Create(this, state, activity.ImplementationId);
    Manager.OnActivityBegin();

    await activity.ExecuteAsync(Manager.Actor, CancellationToken.None);
  }

  private async Task<TState?> GetStateAsync(CancellationToken cancellationToken)
  {
    ConditionalValue<TState> state = await StateManager.TryGetStateAsync<TState>(Constants.ActorStateName, cancellationToken);

    if (state.HasValue)
      return state.Value;

    return null;
  }
}
