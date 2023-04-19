namespace CodeArchitects.Platform.Actors.Messaging;

/// <summary>
/// Represents a message intended for an actor.
/// </summary>
/// <typeparam name="TActorId">The actor id type.</typeparam>
public interface IActorMessage<TActorId>
  where TActorId : notnull
{
  /// <summary>
  /// The id of the actor.
  /// </summary>
  TActorId ActorId { get; }
}
