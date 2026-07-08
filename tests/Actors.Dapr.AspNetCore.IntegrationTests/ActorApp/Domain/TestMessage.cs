using CodeArchitects.Platform.Actors.Messaging;

namespace ActorApp.Domain;

public class TestMessage : IActorMessage<Guid>
{
  public Guid ActorId { get; set; }

  public string Output { get; set; } = "";
}
