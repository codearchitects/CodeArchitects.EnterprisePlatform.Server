using CodeArchitects.Platform.Actors;

namespace ActorApp.OldSkool;

public class TrafficLightState : IActorIdSource<Guid>
{
  public Guid Id { get; private set; }
  public LightColor Color { get; set; }
  public int MaxCarsBeforeYellow { get; set; } = 5;
  public int CarsBeforeYellow { get; set; }
  public DateTime TurnsGreenAt { get; set; }

  Guid IActorIdSource<Guid>.GetActorId()
  {
    return Id;
  }

  void IActorIdSource<Guid>.SetActorId(Guid actorId)
  {
    Id = actorId;
  }
}
