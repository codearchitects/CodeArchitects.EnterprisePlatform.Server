using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Scheduling;

namespace ActorApp.Domain;

[Actor, Virtual]
public abstract class TrafficLight : ITrafficLight
{
  protected static readonly ScheduleId _turnGreenSchedule = ScheduleId.New("TurnGreen");
  protected static readonly ScheduleId _turnRedSchedule = ScheduleId.New("TurnRed");
  protected static readonly ScheduleId _turnYellowSchedule = ScheduleId.New("TurnYellow");

  [State] protected readonly TrafficLightState _state;
  protected readonly IActorContext<TrafficLight> _context;
  
  public TrafficLight(TrafficLightState state, IActorContext<TrafficLight> context)
  {
    _state = state;
    _context = context;
  }

  protected abstract ILogger<TrafficLight> Logger { get; }

  public abstract Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default);

  public abstract ValueTask<string> GetLightColorAsync(CancellationToken cancellationToken = default);

  public abstract Task StartAsync(CancellationToken cancellationToken = default);

  public abstract Task StopAsync(CancellationToken cancellationToken = default);

  protected virtual async Task TurnRedAsync(string reason)
  {
    Logger.LogInformation("Turning red. Reason: {reason}.", reason);

    _context.Become<RedTrafficLight>();

    TimeSpan timer = TimeSpan.FromSeconds(30);
    _state.TurnsGreenAt = DateTime.Now + timer;

    await _context.ScheduleAsync(_turnGreenSchedule, self => self.TurnGreenAsync("regular schedule", -2), SchedulingOptions.In(timer));
  }

  protected virtual async Task TurnYellowAsync(string reason)
  {
    Logger.LogInformation("Turning yellow. Reason: {reason}.", reason);

    _context.Become<YellowTrafficLight>();

    await _context.ScheduleAsync(_turnRedSchedule, self => self.TurnRedAsync("regular schedule"), SchedulingOptions.In(10.Seconds()));
  }

  protected virtual async Task TurnGreenAsync(string reason, int maxCarsIncrement)
  {
    _state.MaxCarsBeforeYellow = Math.Max(5, _state.MaxCarsBeforeYellow + maxCarsIncrement);
    _state.CarsBeforeYellow = 0;

    Logger.LogInformation("Turning green. Reason: {reason}. Max cars before yellow: {maxCars}.", reason, _state.MaxCarsBeforeYellow);

    _context.Become<GreenTrafficLight>();

    await _context.ScheduleAsync(_turnYellowSchedule, self => self.TurnYellowAsync("regular schedule"), SchedulingOptions.In(30.Seconds()));
  }
}
