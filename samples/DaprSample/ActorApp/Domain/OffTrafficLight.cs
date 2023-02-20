using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorImplementation<TrafficLight>(IsDefault = true)]
public class OffTrafficLight : TrafficLight
{
  private readonly ILogger<OffTrafficLight> _logger;

  public OffTrafficLight(TrafficLightState state, IActorContext<TrafficLight> context, ILogger<OffTrafficLight> logger)
    : base(state, context)
  {
    _logger = logger;
  }

  protected override ILogger<TrafficLight> Logger => _logger;

  public override Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromException<TrafficLightResponse>(new InvalidOperationException("I was not started!"));
  }

  public override ValueTask<LightColor> GetLightColorAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromException<LightColor>(new InvalidOperationException("I was not started!"));
  }

  public override async Task StartAsync(CancellationToken cancellationToken = default)
  {
    await TurnGreenAsync();
  }

  public override Task StopAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromException(new InvalidOperationException("I am already off!"));
  }
}
