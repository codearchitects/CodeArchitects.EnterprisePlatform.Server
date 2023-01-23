namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorDescriptor
{
  Type InterfaceType { get; }

  Type ImplementationType { get; }

  bool IsVirtual { get; }

  IImplementationDescriptor StartingImplementation { get; }

  IReadOnlyList<IImplementationDescriptor> Implementations { get; }

  IActorIdDescriptor Id { get; }

  IStateDescriptor State { get; }

  IActorFactoryDescriptor Factory { get; }

  IReadOnlyList<IMethodDescriptor> Methods { get; }
}
