using CodeArchitects.Platform.Actors.Metadata;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal abstract class ActorDescriptor : IActorDescriptor
{
  private IStateDescriptor? _state;

  protected ActorDescriptor(Type interfaceType, Type actorType, IActorIdDescriptor id, IActorFactoryDescriptor factory, IImplementationDescriptor baseImplementation)
  {
    InterfaceType = interfaceType;
    ActorType = actorType;
    Id = id;
    Factory = factory;
    BaseImplementation = baseImplementation;
  }

  public abstract bool IsPolymorphic { get; }

  public IImplementationDescriptor BaseImplementation { get; }

  public abstract IImplementationDescriptor DefaultImplementation { get; }

  public abstract IReadOnlyList<IImplementationDescriptor> Implementations { get; }

  public Type InterfaceType { get; }

  public Type ActorType { get; }

  [AllowNull]
  public IStateDescriptor State
  {
    get => _state ?? NoStateDescriptor.Instance;
    set => _state = value;
  }

  public IActorIdDescriptor Id { get; }

  public IActorFactoryDescriptor Factory { get; }

  public static ActorDescriptor Create(IActorMetadata actorMetadata, IStateDescriptor state, bool isPolymorphic)
  {
    Type interfaceType = GetInterfaceType(actorMetadata);
    CheckInterfaceType(interfaceType, actorMetadata.ActorType);

    ImplementationDescriptor baseImplementation = ImplementationDescriptor.Create(actorMetadata, actorMetadata.BaseImplementation, interfaceType);

    ActorIdDescriptor id = ActorIdDescriptor.Create(actorMetadata, baseImplementation);
    ActorFactoryDescriptor factory = ActorFactoryDescriptor.Create(actorMetadata, interfaceType, state, id);

    ActorDescriptor actor = isPolymorphic
      ? PolymorphicActorDescriptor.Create(actorMetadata, interfaceType, id, factory, baseImplementation)
      : new OrdinaryActorDescriptor(interfaceType, actorMetadata.ActorType, id, factory, baseImplementation);

    actor.State = state;

    return actor;
  }

  private static Type GetInterfaceType(IActorMetadata actorMetadata)
  {
    Type actorType = actorMetadata.ActorType;
    Type? interfaceType = actorMetadata.InterfaceType;
    Type[] implementedInterfaceTypes = actorType.GetInterfaces();

    if (interfaceType is null)
    {
      if (implementedInterfaceTypes.Length == 0)
        throw InvalidActorException.MissingActorInterface(actorType);

      if (implementedInterfaceTypes.Length != 1)
        throw InvalidActorException.AmbiguousActorInterface(actorType);

      return implementedInterfaceTypes[0];
    }

    if (!interfaceType.IsInterface)
      throw InvalidActorException.InterfaceTypeIsNotAnInterface(actorType, interfaceType);

    foreach (Type implementedInterfaceType in implementedInterfaceTypes)
    {
      if (interfaceType == implementedInterfaceType)
        return implementedInterfaceType;
    }

    throw InvalidActorException.InterfaceNotImplemented(actorType);
  }

  private static void CheckInterfaceType(Type interfaceType, Type actorType)
  {
    if (interfaceType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Length != 0)
      throw InvalidActorException.PropertiesAreNotSupported(actorType, interfaceType);

    if (interfaceType.GetEvents(BindingFlags.Instance | BindingFlags.Public).Length != 0)
      throw InvalidActorException.EventsAreNotSupported(actorType, interfaceType);
  }
}
