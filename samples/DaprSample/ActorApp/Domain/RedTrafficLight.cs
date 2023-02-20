using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Scheduling;

namespace ActorApp.Domain;

[ActorImplementation<TrafficLight>]
public class RedTrafficLight : ActiveTrafficLight
{
  private readonly ILogger<RedTrafficLight> _logger;

  public RedTrafficLight(TrafficLightState state, IActorContext<TrafficLight> context, ILogger<RedTrafficLight> logger)
    : base(state, context)
  {
    _logger = logger;
  }

  protected override ILogger<TrafficLight> Logger => _logger;

  protected override ScheduleId ChangeColorSchedule => _turnGreenSchedule;

  public override Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromResult(new TrafficLightResponse
    {
      CanCross = false,
      TurnsGreenAt = _state.TurnsGreenAt
    });
  }

  public override ValueTask<string> GetLightColorAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult("red");
  }

  protected override Task TurnRedAsync(string reason)
  {
    return Task.FromException(new InvalidOperationException("I am already red!"));
  }
}
