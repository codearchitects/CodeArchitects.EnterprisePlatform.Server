using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorImplementation<TrafficLight>]
public class RedTrafficLight : TrafficLight
{
  private readonly ILogger<RedTrafficLight> _logger;

  public RedTrafficLight(TrafficLightState state, IActorContext<TrafficLight> context, ILogger<RedTrafficLight> logger)
    : base(state, context)
  {
    _logger = logger;
  }

  protected override ILogger<TrafficLight> Logger => _logger;

  public override Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromResult(new TrafficLightResponse
    {
      CanCross = false,
      TurnsGreenAt = _state.TurnsGreenAt
    });
  }

  public override ValueTask<LightColor> GetLightColorAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult(LightColor.Red);
  }

  protected override Task TurnRedAsync()
  {
    return Task.FromException(new InvalidOperationException("I am already red!"));
  }
}
