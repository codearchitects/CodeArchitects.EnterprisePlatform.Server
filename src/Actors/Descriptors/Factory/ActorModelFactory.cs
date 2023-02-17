using CodeArchitects.Platform.Actors.Descriptors.Implementation;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal class ActorModelFactory
{
  protected readonly StateTypeBuilder _stateTypeBuilder;
  protected readonly ActivityTypeBuilder _activityTypeBuilder;
  protected readonly Dictionary<Type, ActorDescriptorFactory> _actorDescriptorFactories;
  private IActorModel? _actorModel;

  protected ActorModelFactory()
  {
    _stateTypeBuilder = new(DynamicAssembly.Module, new ReflectionILGeneratorProvider());
    _activityTypeBuilder = new(DynamicAssembly.Module, new ReflectionILGeneratorProvider());
    _actorDescriptorFactories = new();
  }

  public virtual IActorModel CreateModel()
  {
    if (_actorModel is null)
    {
      ActorModel model = new();
      foreach (ActorDescriptorFactory factory in _actorDescriptorFactories.Values)
      {
        IActorDescriptor actor = factory.CreateDescriptor();
        model.AddActor(actor);
      }

      _actorModel = model;
    }

    return _actorModel;
  }
}
