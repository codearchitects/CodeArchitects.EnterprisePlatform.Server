using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal class ReflectionMetadataContext
{
  private readonly Dictionary<Type, ReflectionMetadataSource> _sources;

  public ReflectionMetadataContext()
  {
    _sources = new();
  }

  public IEnumerable<ReflectionMetadataSource> MetadataSources => _sources.Values;

  public void AddActor(Type actorType, Type factoryType, IReadOnlyCollection<Type> implementationTypes)
  {
    if (!IsActor(actorType, out IActorAttribute? actorAttribute))
      throw new ArgumentException($"'{nameof(actorType)}' must be a type which defines the 'Actor' attribute.", nameof(actorType));

    if (!IsActorFactory(factoryType, out IActorFactoryAttribute? actorFactoryAttribute) || actorFactoryAttribute.ActorType != actorType)
      throw new ArgumentException($"'{nameof(factoryType)}' must be a type which defines the 'ActorFactory' attribute, which actor type must be equal to '{nameof(actorType)}'", nameof(factoryType));

    foreach (Type implementationType in implementationTypes)
    {
      if (!IsActorImplementation(implementationType))
        throw new ArgumentException($"'{nameof(implementationTypes)}' should contain only types which defines the 'ActorImplementation' attribute.", nameof(implementationTypes));
    }

    _sources[actorType] = new ReflectionMetadataSource(actorType, factoryType, actorAttribute, implementationTypes);
  }

  public void AddAssembly(Assembly assembly)
  {
    foreach (Type type in assembly.GetTypes())
    {
      if (IsActor(type, out IActorAttribute? actorAttribute))
      {
        _sources.Add(type, new ReflectionMetadataSource(type, null, actorAttribute, new List<Type>()));
      }
    }

    foreach (Type type in assembly.GetTypes())
    {
      if (IsActorFactory(type, out IActorFactoryAttribute? actorFactoryAttribute))
      {
        Type actorType = actorFactoryAttribute.ActorType;

        if (!_sources.TryGetValue(actorType, out ReflectionMetadataSource? source))
          throw InvalidActorException.ActorTypeIsNotAnActor(actorType, type);

        if (source.FactoryType is not null)
          throw InvalidActorException.AmbiguousActorFactoryType(actorType);

        _sources[actorType] = source with { FactoryType = type };

        continue;
      }

      if (IsActorImplementation(type))
      {
        if (!TryGetReflectionMetadataSource(type, out ReflectionMetadataSource? source))
          throw InvalidActorException.InvalidImplementation(type); // TODO: Try move in descriptors

        _sources[source.ActorType] = source with { ImplementationTypes = new List<Type>(source.ImplementationTypes) { type } };
      }
    }
  }

  private static bool IsActor(Type type, [NotNullWhen(true)] out IActorAttribute? actorAttribute)
  {
    actorAttribute = null;

    foreach (object attribute in type.GetCustomAttributes())
    {
      if (typeof(IActorAttribute).IsInstanceOfType(attribute))
      {
        if (actorAttribute is not null)
          throw InvalidActorException.DuplicateActorAttribute(type);

        actorAttribute = (IActorAttribute)attribute;
      }
    }

    return actorAttribute is not null;
  }

  private static bool IsActorFactory(Type type, [NotNullWhen(true)] out IActorFactoryAttribute? actorFactoryAttribute)
  {
    actorFactoryAttribute = null;

    foreach (object attribute in type.GetCustomAttributes())
    {
      if (typeof(IActorFactoryAttribute).IsInstanceOfType(attribute))
      {
        if (actorFactoryAttribute is not null)
          throw InvalidActorException.DuplicateActorFactoryAttribute(actorFactoryAttribute.ActorType, type);

        actorFactoryAttribute = (IActorFactoryAttribute)attribute;
      }
    }

    return actorFactoryAttribute is not null;
  }

  private static bool IsActorImplementation(Type type)
  {
    return type.IsDefined(typeof(ActorImplementationAttribute));
  }

  private bool TryGetReflectionMetadataSource(Type implementationType, [NotNullWhen(true)] out ReflectionMetadataSource? source)
  {
    Type? type = implementationType.BaseType;

    while (type is not null)
    {
      if (_sources.TryGetValue(type, out source))
        return true;

      type = type.BaseType;
    }

    source = null;
    return false;
  }
}
