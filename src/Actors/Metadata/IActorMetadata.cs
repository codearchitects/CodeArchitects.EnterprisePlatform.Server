namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IActorMetadata
{
  Type? InterfaceType { get; }

  Type ActorType { get; }

  bool IsExplicitVirtual { get; }

  Type? FactoryType { get; }

  IReadOnlyCollection<IStateFieldMetadata> StateFields { get; }

  IImplementationMetadata BaseImplementation { get; }

  IReadOnlyCollection<IImplementationMetadata> Implementations { get; }
}
