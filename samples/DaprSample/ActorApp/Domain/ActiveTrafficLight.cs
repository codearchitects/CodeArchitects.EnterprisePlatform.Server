using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Scheduling;

namespace ActorApp.Domain;

public abstract class ActiveTrafficLight : TrafficLight
{
  protected ActiveTrafficLight(TrafficLightState state, IActorContext<TrafficLight> context)
    : base(state, context)
  {
  }

  protected abstract ScheduleId ChangeColorSchedule { get; }

  public override Task StartAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromException(new InvalidOperationException("I was already started!"));
  }

  public override async Task StopAsync(CancellationToken cancellationToken = default)
  {
    Logger.LogInformation("Traffic light is stopping.");

    await _context.UnscheduleAsync(ChangeColorSchedule, cancellationToken);
    
    _context.Become<InactiveTrafficLight>();
  }
}
