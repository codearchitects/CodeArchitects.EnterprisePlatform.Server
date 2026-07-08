namespace CodeArchitects.Platform.Actors.Metadata.Builder;

/// <summary>
/// A builder that can be used to configure an actor's state component.
/// </summary>
/// <typeparam name="TActor">The actor type.</typeparam>
/// <typeparam name="TStateComponent">The state component type.</typeparam>
public interface IStateComponentBuilder<TActor, TStateComponent>
  where TActor : class
{
  /// <summary>
  /// Specifies a default value for the component.
  /// </summary>
  /// <param name="value">The component's default value.</param>
  /// <returns>The same builder.</returns>
  IStateComponentBuilder<TActor, TStateComponent> HasDefaultValue(TStateComponent value);

  /// <summary>
  /// Specifies a default value factory for the component.
  /// </summary>
  /// <param name="valueFactory">The component's default value factory.</param>
  /// <returns>The same builder.</returns>
  IStateComponentBuilder<TActor, TStateComponent> HasDefaultValueFactory(Func<TStateComponent> valueFactory);

  /// <summary>
  /// Indicates whether a component is also the id of the actor.
  /// </summary>
  /// <param name="isActorId"><see langword="true"/> if the component is also the actor id, otherwise <see langword="false"/>.</param>
  /// <returns>The same builder.</returns>
  IStateComponentBuilder<TActor, TStateComponent> IsActorId(bool isActorId = true);
}
