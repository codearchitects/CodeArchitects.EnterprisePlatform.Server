using CodeArchitects.Platform.Actors.Descriptors.Factory;

namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

internal abstract class ActorConfiguration : ActorModelFactory
{
  private bool _isConfigured;

  protected abstract void Configure();

  public override IActorModel CreateModel()
  {
    if (!_isConfigured)
    {
      _isConfigured = true;
      Configure();
    }
    return base.CreateModel();
  }

  protected void Actor<TActor>(Action<IActorMetadataBuilder<TActor>> configure)
    where TActor : class
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    ActorMetadataBuilder<TActor> builder;
    if (_actorDescriptorFactories.TryGetValue(typeof(TActor), out var obj))
    {
      builder = (ActorMetadataBuilder<TActor>)obj;
    }
    else
    {
      builder = new(_stateTypeBuilder, _activityTypeBuilder);
      _actorDescriptorFactories.Add(typeof(TActor), builder);
    }

    configure(builder);
  }
}
