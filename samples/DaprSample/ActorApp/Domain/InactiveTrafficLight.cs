using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorImplementation<TrafficLight>(IsDefault = true)]
public class InactiveTrafficLight : TrafficLight
{
  private readonly ILogger<InactiveTrafficLight> _logger;

  public InactiveTrafficLight(TrafficLightState state, IActorContext<TrafficLight> context, ILogger<InactiveTrafficLight> logger)
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
    await TurnGreenAsync("Traffic light is starting", 0);
  }

  public override Task StopAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromException(new InvalidOperationException("I am already off!"));
  }
}
