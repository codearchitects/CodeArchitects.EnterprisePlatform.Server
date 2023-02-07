using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
using Dapr.Actors.Runtime;
using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

internal class DaprActorHost<TActor, TState> : Actor, IActorHost<TActor, TState>, IRemindable
  where TActor : class
  where TState : ActorState
{
  private readonly IActorManager<TActor, TState> _manager;
  private readonly IImplementationFactory<TActor, TState> _factory;
  private TActor? _actor;
  private TState? _state;

  public DaprActorHost(ActorHost host, IActorManager<TActor, TState> manager, IImplementationFactory<TActor, TState> factory)
    : base(host)
  {
    _manager = manager;
    _factory = factory;
  }

  public TActor Actor => _actor ?? throw new InvalidOperationException("Host was not initialized.");

  public TState State => _state ?? throw new InvalidOperationException("Host was not initialized.");

  public string ActorId => Id.GetId();

  protected override async Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
  {
    if (actorMethodContext.CallType is not ActorCallType.ActorInterfaceMethod)
      return;

    _state = await GetStateAsync(CancellationToken.None);
    _actor = _factory.Create(this, _state);
  }

  protected override Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
  {
    return SaveStateAsync(CancellationToken.None);
  }

  public Task ScheduleAsync(Activity<TActor> activity, SchedulingOptions options, CancellationToken cancellationToken)
  {
    byte[] payload = JsonSerializer.SerializeToUtf8Bytes(activity, _manager.ActivityType, _manager.JsonSerializerOptions);

    return RegisterReminderAsync(options.ScheduleId, payload, options.Timer, options.Period);
  }

  public Task UnscheduleAsync(ScheduleId id, CancellationToken cancellationToken)
  {
    return UnregisterReminderAsync(id);
  }

  public async Task ReceiveReminderAsync(string reminderName, byte[] payload, TimeSpan dueTime, TimeSpan period)
  {
    Activity<TActor> activity = (Activity<TActor>)JsonSerializer.Deserialize(payload, _manager.ActivityType, _manager.JsonSerializerOptions)!;

    _state = await GetStateAsync(CancellationToken.None);
    _actor = _factory.Create(this, _state, activity.ImplementationId);

    await activity.ExecuteAsync(_actor, CancellationToken.None);
  }

  private async Task<TState> GetStateAsync(CancellationToken cancellationToken)
  {
    ConditionalValue<TState> state = await StateManager.TryGetStateAsync<TState>(Constants.ActorStateName, cancellationToken);

    if (state.HasValue)
      return state.Value;

    return _manager.DefaultState;
  }

  private Task SaveStateAsync(CancellationToken cancellationToken)
  {
    _manager.UpdateState(_actor!, _state!);
    return StateManager.SetStateAsync(Constants.ActorStateName, _state, cancellationToken);
  }
}
