using CodeArchitects.Platform.Actors.Messaging;
using CodeArchitects.Platform.Messaging;

namespace ActorApp.Domain;

[Message]
public class TurnOffCommand : IActorMessage<Guid>
{
  public Guid ActorId { get; set; }

  public string Reason { get; set; } = "no reason";
}
