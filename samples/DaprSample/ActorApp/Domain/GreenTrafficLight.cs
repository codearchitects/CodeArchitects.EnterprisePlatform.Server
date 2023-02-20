using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Scheduling;

namespace ActorApp.Domain;

[ActorImplementation<TrafficLight>]
public class GreenTrafficLight : ActiveTrafficLight
{
  private readonly ILogger<GreenTrafficLight> _logger;

  public GreenTrafficLight(TrafficLightState state, IActorContext<TrafficLight> context, ILogger<GreenTrafficLight> logger)
    : base(state, context)
  {
    _logger = logger;

    context.RegisterBinding<GreenTrafficLight>(binding => binding
      .WithPreCondition(self => self._state.CarsBeforeYellow < self._state.MaxCarsBeforeYellow)
      .WithPostCondition(self => self._state.CarsBeforeYellow >= self._state.MaxCarsBeforeYellow)
      .IsEnabled()
      .BindTo((self, cancellationToken) => self.OnTooManyCarsAsync()));
  }

  protected override ILogger<TrafficLight> Logger => _logger;

  protected override ScheduleId ChangeColorSchedule => _greenToYellowSchedule;

  public override Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default)
  {
    _state.CarsBeforeYellow++;

    return Task.FromResult(new TrafficLightResponse
    {
      CanCross = true
    });
  }

  public override ValueTask<LightColor> GetLightColorAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult(LightColor.Green);
  }

  protected override Task TurnGreenAsync(string reason, int maxCarsIncrement)
  {
    return Task.FromException(new InvalidOperationException("I am already green!"));
  }

  private async Task OnTooManyCarsAsync()
  {
    _state.MaxCarsBeforeYellow += 3;

    await _context.UnscheduleAsync(_greenToYellowSchedule);

    await TurnYellowAsync("Too many cars have passed");
  }
}
