namespace CodeArchitects.Platform.Actors.Metadata.Builder;

/// <summary>
/// A builder that can be used to configure an actor's state component.
/// </summary>
/// <typeparam name="TActor">The actor type.</typeparam>
/// <typeparam name="TState">The state component type.</typeparam>
public interface IStateComponentBuilder<TActor, TState>
  where TActor : class
{
  /// <summary>
  /// Specifies a default value for the component.
  /// </summary>
  /// <param name="value">The component's default value.</param>
  /// <returns>The same builder.</returns>
  IStateComponentBuilder<TActor, TState> HasDefaultValue(TState value);

  /// <summary>
  /// Indicates whether a component is also the id of the actor.
  /// </summary>
  /// <param name="isActorId"><see langword="true"/> if the component is also the actor id, otherwise <see langword="false"/>.</param>
  /// <returns>The same builder.</returns>
  IStateComponentBuilder<TActor, TState> IsActorId(bool isActorId = true);
}
