using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Scheduling;

namespace ActorApp.Domain;

[Actor, Virtual]
public abstract class TrafficLight : ITrafficLight
{
  protected static readonly ScheduleId _turnYellowSchedule = ScheduleId.New();

  [State] protected readonly TrafficLightState _state;
  protected readonly IActorContext<TrafficLight> _context;
  
  public TrafficLight(TrafficLightState state, IActorContext<TrafficLight> context)
  {
    _state = state;
    _context = context;
  }

  protected abstract ILogger<TrafficLight> Logger { get; }

  public abstract Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default);

  public abstract ValueTask<LightColor> GetLightColorAsync(CancellationToken cancellationToken = default);

  public virtual Task StartAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromException(new InvalidOperationException("I was already started!"));
  }

  public virtual Task StopAsync(CancellationToken cancellationToken = default)
  {
    _context.Become<OffTrafficLight>();
    return Task.CompletedTask;
  }

  protected virtual async Task TurnRedAsync()
  {
    Logger.LogInformation("Turning red...");

    _context.Become<RedTrafficLight>();

    TimeSpan timer = 1.Minutes();
    _state.TurnsGreenAt = DateTime.Now + timer;

    await _context.ScheduleAsync(self => self.TurnGreenAsync(), SchedulingOptions.In(timer));
  }

  protected virtual async Task TurnYellowAsync()
  {
    Logger.LogInformation("Turning yellow...");

    _context.Become<YellowTrafficLight>();

    await _context.ScheduleAsync(self => self.TurnRedAsync(), SchedulingOptions.In(10.Seconds()));
  }

  protected virtual async Task TurnGreenAsync()
  {
    Logger.LogInformation("Turning green...");

    _state.CarCount = 5;

    _context.Become<GreenTrafficLight>();

    await _context.ScheduleAsync(_turnYellowSchedule, self => self.TurnYellowAsync(), SchedulingOptions.In(30.Seconds()));
  }
}
