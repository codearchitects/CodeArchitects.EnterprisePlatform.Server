namespace CodeArchitects.Platform.Actors.Messaging;

public interface IActorMessage<TActorId>
  where TActorId : notnull
{
  TActorId ActorId { get; }
}
