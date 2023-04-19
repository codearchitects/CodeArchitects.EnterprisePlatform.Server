namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal interface IReflectionMetadataContext
{
  Type? GetFactoryType(Type actorType);

  IEnumerable<Type> GetImplementationTypes(Type actorType);
}