using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Reflection;

internal class ReflectionMetadataContext : ActorModelFactory, IReflectionMetadataContext
{
  private readonly Dictionary<Type, Type> _factoryTypes;
  private readonly Dictionary<Type, HashSet<Type>> _implementationTypes;
  private readonly HashSet<Assembly> _scannedAssemblies;

  public ReflectionMetadataContext()
  {
    _factoryTypes = new();
    _implementationTypes = new();
    _scannedAssemblies = new();
  }

  public Type? GetFactoryType(Type actorType)
  {
    _ = _factoryTypes.TryGetValue(actorType, out Type? factoryType);
    return factoryType;
  }

  public IEnumerable<Type> GetImplementationTypes(Type actorType)
  {
    _ = _implementationTypes.TryGetValue(actorType, out HashSet<Type>? implementationTypes);
    return implementationTypes ?? Enumerable.Empty<Type>();
  }

  public void AddActor(Type actorType)
  {
    if (!actorType.IsClass)
      throw new ArgumentException("An actor type must be a class.");

    if (IsActor(actorType, out IActorAttribute? actorAttribute) && _scannedAssemblies.Contains(actorType.Assembly))
      return;

    actorAttribute ??= new ActorAttribute();

    _factories[actorType] = delegate (IStateTypeBuilder stateTypeBuilder, IActivityTypeBuilder activityTypeBuilder)
    {
      Type descriptorFactoryType = typeof(ReflectionActorDescriptorFactory<>).MakeGenericType(actorType);
      return (ActorDescriptorFactory)Activator.CreateInstance(descriptorFactoryType, new object?[] { stateTypeBuilder, activityTypeBuilder, this, actorAttribute })!;
    };

    foreach (Type type in actorType.Assembly.GetTypes())
    {
      if (IsActorFactory(type, out IActorFactoryAttribute? actorFactoryAttribute) && actorFactoryAttribute.ActorType == actorType)
      {
        if (_factoryTypes.ContainsKey(actorType))
          throw InvalidActorException.AmbiguousActorFactoryType(actorType);

        _factoryTypes.Add(actorType, type);

        continue;
      }

      if (IsActorImplementation(type, out IActorImplementationAttribute? actorImplementationAttribute) && actorImplementationAttribute.ActorType == actorType)
      {
        if (!actorType.IsAssignableFrom(type))
          throw InvalidActorException.InvalidImplementation(type);

        if (!_implementationTypes.TryGetValue(actorType, out HashSet<Type> types))
        {
          types = new();
          _implementationTypes.Add(actorType, types);
        }

        types.Add(type);
      }
    }
  }

  public void AddAssembly(Assembly assembly)
  {
    if (_scannedAssemblies.Contains(assembly))
      return;

    _scannedAssemblies.Add(assembly);

    foreach (Type type in assembly.GetTypes())
    {
      if (IsActor(type, out IActorAttribute? actorAttribute))
      {
        _factories[type] = delegate (IStateTypeBuilder stateTypeBuilder, IActivityTypeBuilder activityTypeBuilder)
        {
          Type descriptorFactoryType = typeof(ReflectionActorDescriptorFactory<>).MakeGenericType(type);
          return (ActorDescriptorFactory)Activator.CreateInstance(descriptorFactoryType, new object?[] { stateTypeBuilder, activityTypeBuilder, this, actorAttribute })!;
        };
      }

      if (IsActorFactory(type, out IActorFactoryAttribute? actorFactoryAttribute))
      {
        Type actorType = actorFactoryAttribute.ActorType;

        if (_factoryTypes.ContainsKey(actorType))
          throw InvalidActorException.AmbiguousActorFactoryType(actorType);

        _factoryTypes.Add(actorType, type);

        continue;
      }

      if (IsActorImplementation(type, out IActorImplementationAttribute? actorImplementationAttribute))
      {
        Type actorType = actorImplementationAttribute.ActorType;
        if (!actorType.IsAssignableFrom(type))
          throw InvalidActorException.InvalidImplementation(type);

        if (!_implementationTypes.TryGetValue(actorType, out HashSet<Type> types))
        {
          types = new();
          _implementationTypes.Add(actorType, types);
        }

        types.Add(type);
      }
    }
  }

  private static bool IsActor(Type type, [NotNullWhen(true)] out IActorAttribute? actorAttribute)
  {
    actorAttribute = null;

    foreach (object attribute in type.GetCustomAttributes(inherit: false))
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

    foreach (object attribute in type.GetCustomAttributes(inherit: false))
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

  private static bool IsActorImplementation(Type type, [NotNullWhen(true)] out IActorImplementationAttribute? actorImplementationAttribute)
  {
    actorImplementationAttribute = null;

    foreach (object attribute in type.GetCustomAttributes(inherit: false))
    {
      if (typeof(IActorImplementationAttribute).IsInstanceOfType(attribute))
      {
        if (actorImplementationAttribute is not null)
          throw InvalidActorException.DuplicateImplementationAttribute(type);

        actorImplementationAttribute = (IActorImplementationAttribute)attribute;
      }
    }

    return actorImplementationAttribute is not null;
  }
}
