using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IActorMetadata
{
  Type? InterfaceType { get; }

  Type ActorType { get; }

  bool IsExplicitVirtual { get; }

  Type? FactoryType { get; }

  ConstructorInfo Constructor { get; }

  IReadOnlyCollection<IStateFieldMetadata> StateFields { get; }

  IReadOnlyCollection<IImplementationMetadata> Implementations { get; }
}
