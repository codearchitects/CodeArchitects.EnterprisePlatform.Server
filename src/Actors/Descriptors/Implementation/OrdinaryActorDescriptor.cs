using CodeArchitects.Platform.Actors.Metadata;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class OrdinaryActorDescriptor : ActorDescriptor
{
  public OrdinaryActorDescriptor(Type interfaceType, Type actorType, IActorIdDescriptor id, IActorFactoryDescriptor factory, IImplementationDescriptor defaultImplementation)
    : base(interfaceType, actorType, id, factory)
  {
    DefaultImplementation = defaultImplementation;
  }

  public override bool IsPolymorphic => false;

  public override IImplementationDescriptor DefaultImplementation { get; }

  public override IReadOnlyList<IImplementationDescriptor> Implementations => new[] { DefaultImplementation };

  public override IConstructorDescriptor Constructor => DefaultImplementation.Constructor;


  public static OrdinaryActorDescriptor Create(IActorMetadata actorMetadata, IActorIdDescriptor id, IActorFactoryDescriptor factory, Type interfaceType)
  {
    ImplementationDescriptor implementation = ImplementationDescriptor.Create(actorMetadata, actorMetadata.Implementations.Single(), interfaceType);
    
    return new(interfaceType, actorMetadata.ActorType, id, factory, implementation);
  }
}
