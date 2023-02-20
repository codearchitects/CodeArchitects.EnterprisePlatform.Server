namespace CodeArchitects.Platform.Actors;

public interface IActorIdSource<TActorId>
  where TActorId : notnull
{
  TActorId GetActorId();

  void SetActorId(TActorId actorId);
}
