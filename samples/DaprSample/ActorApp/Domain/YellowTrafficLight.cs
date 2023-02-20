using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Scheduling;

namespace ActorApp.Domain;

[ActorImplementation<TrafficLight>]
public class YellowTrafficLight : ActiveTrafficLight
{
  private readonly ILogger<YellowTrafficLight> _logger;

  public YellowTrafficLight(TrafficLightState state, IActorContext<TrafficLight> context, ILogger<YellowTrafficLight> logger)
    : base(state, context)
  {
    _logger = logger;
  }

  protected override ILogger<TrafficLight> Logger => _logger;

  protected override ScheduleId ChangeColorSchedule => _yellowToRedSchedule;

  public override Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromResult(new TrafficLightResponse
    {
      CanCross = true
    });
  }

  public override ValueTask<LightColor> GetLightColorAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult(LightColor.Yellow);
  }

  protected override Task TurnYellowAsync(string reason)
  {
    return Task.FromException(new InvalidOperationException("I am already yellow!"));
  }
}
