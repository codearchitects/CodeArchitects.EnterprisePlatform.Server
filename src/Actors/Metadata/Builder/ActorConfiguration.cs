using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

/// <summary>
/// Class used to configure actors with a fluent API.
/// </summary>
public abstract class ActorConfiguration : ActorModelFactory
{
  /// <summary>
  /// Configures the actor model by specifying what actors are registered.
  /// </summary>
  protected internal abstract void Configure();

  /// <summary>
  /// Registers an actor within the application.
  /// </summary>
  /// <typeparam name="TActor">The type of the actor to register.</typeparam>
  /// <param name="configure">An action used to fluently configure the actor.</param>
  protected void Actor<TActor>(Action<IActorMetadataBuilder<TActor>> configure)
    where TActor : class
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    _factories[typeof(TActor)] = delegate (IStateTypeBuilder stateTypeBuilder, IActivityTypeBuilder activityTypeBuilder)
    {
      ActorMetadataBuilder<TActor> builder = new(stateTypeBuilder, activityTypeBuilder);
      configure(builder);

      return builder;
    };
  }
}
