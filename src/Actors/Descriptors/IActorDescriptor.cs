namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorDescriptor
{
  Type InterfaceType { get; }

  Type ActorType { get; }

  bool IsPolymorphic { get; }

  IImplementationDescriptor BaseImplementation { get; }

  IImplementationDescriptor DefaultImplementation { get; }

  IReadOnlyList<IImplementationDescriptor> Implementations { get; }

  IActorIdDescriptor Id { get; }

  IStateDescriptor State { get; }

  IActorFactoryDescriptor Factory { get; }
}
