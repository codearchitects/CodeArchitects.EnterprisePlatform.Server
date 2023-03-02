namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Interface used to retrieve the actor id from a state component.
/// </summary>
/// <typeparam name="TActorId">The actor id type.</typeparam>
public interface IActorIdSource<TActorId>
  where TActorId : notnull
{
  /// <summary>
  /// Gets the actor id.
  /// </summary>
  /// <returns>The actor id.</returns>
  TActorId GetActorId();

  /// <summary>
  /// Sets the actor id.
  /// </summary>
  /// <param name="actorId">The value of the actor id.</param>
  void SetActorId(TActorId actorId);
}
