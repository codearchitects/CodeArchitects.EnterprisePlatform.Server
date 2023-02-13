using System.Reflection;
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

  IReadOnlyList<FieldInfo> StateFields { get; }

  IImplementationDescriptor BaseImplementation { get; }

  IImplementationDescriptor DefaultImplementation { get; }

  IReadOnlyList<IImplementationDescriptor> Implementations { get; }

  IReadOnlyList<IMethodDescriptor> Activities { get; }

  IActorIdDescriptor Id { get; }

  IStateDescriptor State { get; }

  IActorFactoryDescriptor Factory { get; }

  int GetImplementationId(Type implementationType);
}
