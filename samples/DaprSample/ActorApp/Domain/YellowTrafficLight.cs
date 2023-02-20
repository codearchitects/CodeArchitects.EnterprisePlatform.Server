using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorImplementation<TrafficLight>]
public class YellowTrafficLight : TrafficLight
{
  private readonly ILogger<YellowTrafficLight> _logger;

  public YellowTrafficLight(TrafficLightState state, IActorContext<TrafficLight> context, ILogger<YellowTrafficLight> logger)
    : base(state, context)
  {
    _logger = logger;
  }

  protected override ILogger<TrafficLight> Logger => _logger;

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

  protected override Task TurnYellowAsync()
  {
    return Task.FromException(new InvalidOperationException("I am already yellow!"));
  }
}
