using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Bindings;

namespace ActorApp.Domain;

[ActorImplementation<TrafficLight>]
public class GreenTrafficLight : TrafficLight
{
  private readonly ILogger<GreenTrafficLight> _logger;

  public GreenTrafficLight(TrafficLightState state, IActorContext<TrafficLight> context, ILogger<GreenTrafficLight> logger)
    : base(state, context)
  {
    _logger = logger;

    BindingId carCountBindingId = context.RegisterBinding<GreenTrafficLight>(binding => binding
      .WithPreCondition(self => self._state.CarCount > 0)
      .WithPostCondition(self => self._state.CarCount == 0)
      .IsEnabled()
      .BindTo((self, cancellationToken) => self.OnTooManyCarsAsync()));
  }

  protected override ILogger<TrafficLight> Logger => _logger;

  public override Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default)
  {
    _state.CarCount--;

    return Task.FromResult(new TrafficLightResponse
    {
      CanCross = true
    });
  }

  public override ValueTask<LightColor> GetLightColorAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult(LightColor.Green);
  }

  protected override Task TurnGreenAsync()
  {
    return Task.FromException(new InvalidOperationException("I am already green!"));
  }

  private async Task OnTooManyCarsAsync()
  {
    _logger.LogWarning("Turning yellow because too many cars have passed.");

    await _context.UnscheduleAsync(_turnYellowSchedule);

    await TurnYellowAsync();
  }
}
