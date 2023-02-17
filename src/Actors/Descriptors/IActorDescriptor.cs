using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorDescriptor
{
  Type InterfaceType { get; }

  Type ActorType { get; }

  Type ActivityBaseType { get; }

  bool IsPolymorphic { get; }

  bool IsVirtual { get; }

  JsonSerializerOptions JsonSerializerOptions { get; }

  IImplementationDescriptor BaseImplementation { get; }

  IImplementationDescriptor DefaultImplementation { get; }

  IReadOnlyCollection<IImplementationDescriptor> Implementations { get; }

  IReadOnlyCollection<IMethodDescriptor> Methods { get; }

  IReadOnlyCollection<IMethodDescriptor> Activities { get; }

  IActorIdDescriptor Id { get; }

  IStateDescriptor State { get; }

  IActorFactoryDescriptor Factory { get; }

  IImplementationDescriptor GetImplementation(Type implementationType);
}
