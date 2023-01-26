namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal record ReflectionMetadataSource(
  Type ActorType,
  Type? FactoryType,
  IActorAttribute ActorAttribute,
  IReadOnlyCollection<Type> ImplementationTypes);