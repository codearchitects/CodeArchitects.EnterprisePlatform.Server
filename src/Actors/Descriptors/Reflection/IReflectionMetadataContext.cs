namespace CodeArchitects.Platform.Actors.Descriptors.Reflection;

internal interface IReflectionMetadataContext
{
  Type? GetFactoryType(Type actorType);

  IEnumerable<Type> GetImplementationTypes(Type actorType);
}