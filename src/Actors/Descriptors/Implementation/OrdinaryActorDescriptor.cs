namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class OrdinaryActorDescriptor : ActorDescriptor
{
  public OrdinaryActorDescriptor(Type interfaceType, Type actorType, IActorIdDescriptor id, IActorFactoryDescriptor factory, IImplementationDescriptor baseImplementation)
    : base(interfaceType, actorType, id, factory, baseImplementation)
  {
  }

  public override bool IsPolymorphic => false;

  public override IImplementationDescriptor DefaultImplementation => BaseImplementation;

  public override IReadOnlyList<IImplementationDescriptor> Implementations => new[] { DefaultImplementation };
}
