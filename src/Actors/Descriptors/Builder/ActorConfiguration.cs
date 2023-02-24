using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

public abstract class ActorConfiguration : ActorModelFactory
{
  protected internal abstract void Configure();

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
