using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

public abstract class ActorConfiguration : ActorModelFactory
{
  private bool _isConfigured;

  protected abstract void Configure();

  internal override IActorModel CreateModel(ModuleBuilder module)
  {
    if (!_isConfigured)
    {
      _isConfigured = true;
      Configure();
    }
    return base.CreateModel(module);
  }

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
