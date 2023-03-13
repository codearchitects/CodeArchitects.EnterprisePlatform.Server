using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

public class VirtualActorState : IActorIdSource<Guid>
{
  public Guid Id { get; private set; }
  public bool ExecuteBinding { get; set; }

  Guid IActorIdSource<Guid>.GetActorId()
  {
    return Id;
  }

  void IActorIdSource<Guid>.SetActorId(Guid actorId)
  {
    Id = actorId;
  }
}
