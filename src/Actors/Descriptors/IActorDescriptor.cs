using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorDescriptor
{
  Type InterfaceType { get; }

  Type ActorType { get; }

  Type ActivityBaseType { get; }

  bool IsPolymorphic { get; }

  bool IsStateless { get; }

  bool IsVirtual { get; }

  IReadOnlyList<FieldInfo> StateFields { get; }

  IImplementationDescriptor BaseImplementation { get; }

  IImplementationDescriptor DefaultImplementation { get; }

  IReadOnlyList<IImplementationDescriptor> Implementations { get; }

  IReadOnlyList<IMethodDescriptor> Activities { get; }

  IActorIdDescriptor Id { get; }

  IStateDescriptor State { get; }

  IActorFactoryDescriptor Factory { get; }
}
