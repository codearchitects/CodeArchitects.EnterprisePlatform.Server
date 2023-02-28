namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public interface IStateComponentBuilder<TActor, TState>
  where TActor : class
{
  IStateComponentBuilder<TActor, TState> HasDefaultValue(TState value);

  IStateComponentBuilder<TActor, TState> IsActorId(bool isActorId = true);
}
