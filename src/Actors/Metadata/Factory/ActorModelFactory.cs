using CodeArchitects.Platform.Actors.Metadata.Implementation;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Emit.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Metadata.Factory;

public class ActorModelFactory
{
  private protected delegate ActorDescriptorFactory ActorDescriptorFactoryFactory(IStateTypeBuilder stateTypeBuilder, IActivityTypeBuilder activityTypeBuilder);

  private protected readonly Dictionary<Type, ActorDescriptorFactoryFactory> _factories;

  private protected ActorModelFactory()
  {
    _factories = new();
  }

  internal virtual IActorModel CreateModel(ModuleBuilder module)
  {
    StateTypeBuilder stateTypeBuilder = new(module, new ReflectionILGeneratorProvider());
    ActivityTypeBuilder activityTypeBuilder = new(module, new ReflectionILGeneratorProvider());

    ActorModel model = new();
    foreach (ActorDescriptorFactoryFactory factory in _factories.Values)
    {
      ActorDescriptorFactory descriptorFactory = factory(stateTypeBuilder, activityTypeBuilder);
      IActorDescriptor actor = descriptorFactory.CreateDescriptor();
      model.AddActor(actor);
    }

    return model;
  }
}
