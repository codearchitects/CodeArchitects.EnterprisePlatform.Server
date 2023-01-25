using CodeArchitects.Platform.Actors.Metadata;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class PolymorphicActorDescriptor : ActorDescriptor
{
  private readonly List<IImplementationDescriptor> _implementations;
  private IImplementationDescriptor? _defaultImplementation;

  public PolymorphicActorDescriptor(Type interfaceType, Type actorType, IActorIdDescriptor id, IActorFactoryDescriptor factory, IConstructorDescriptor constructor)
    : base(interfaceType, actorType, id, factory)
  {
    Constructor = constructor;
    _implementations = new();
  }

  public override bool IsPolymorphic => true;

  public override IImplementationDescriptor DefaultImplementation => _defaultImplementation ?? Implementations[0];

  public override IReadOnlyList<IImplementationDescriptor> Implementations => _implementations;

  public override IConstructorDescriptor Constructor { get; }

  public void AddImplementation(IImplementationDescriptor implementation)
  {
    _implementations.Add(implementation);
  }

  public void AddDefaultImplementation(IImplementationDescriptor implementation)
  {
    if (_defaultImplementation is not null)
      throw InvalidActorException.MultipleDefaultImplementations(ActorType);

    _implementations.Add(implementation);
    _defaultImplementation = implementation;
  }


  public static PolymorphicActorDescriptor Create(IActorMetadata actorMetadata, IActorIdDescriptor id, IActorFactoryDescriptor factory, IConstructorDescriptor constructor, Type interfaceType)
  {
    PolymorphicActorDescriptor actor = new(interfaceType, actorMetadata.ActorType, id, factory, constructor);

    foreach (IImplementationMetadata implementationMetadata in actorMetadata.Implementations)
    {
      ImplementationDescriptor implementation = ImplementationDescriptor.Create(actorMetadata, implementationMetadata, interfaceType);

      if (implementationMetadata.IsDefault)
      {
        actor.AddDefaultImplementation(implementation);
      }
      else
      {
        actor.AddImplementation(implementation);
      }
    }

    return actor;
  }
}
