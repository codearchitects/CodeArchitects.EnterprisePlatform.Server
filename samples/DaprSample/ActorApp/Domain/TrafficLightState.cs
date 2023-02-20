using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

public class TrafficLightState : IActorIdSource<Guid>
{
  public Guid Id { get; private set; }
  public int CarCount { get; set; }
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
