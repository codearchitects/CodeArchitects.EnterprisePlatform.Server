using CodeArchitects.Platform.Actors.Metadata;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ActorFactoryDescriptor : IActorFactoryDescriptor
{
  public ActorFactoryDescriptor(Type factoryType, MethodInfo? createAsyncMethod, MethodInfo getMethod)
  {
    FactoryType = factoryType;
    CreateAsyncMethod = createAsyncMethod;
    GetMethod = getMethod;
  }

  public Type FactoryType { get; }

  public MethodInfo? CreateAsyncMethod { get; }

  public MethodInfo GetMethod { get; }


  public static ActorFactoryDescriptor Create(IActorMetadata actorMetadata, Type interfaceType, IStateDescriptor state, IActorIdDescriptor id)
  {
    Type actorType = actorMetadata.ActorType;

    if (actorMetadata.FactoryType is not Type factoryType)
      throw InvalidActorException.MissingActorFactoryType(actorType);

    int methodCount = state.IsVirtual ? 1 : 2;
    if (factoryType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Length != methodCount)
      throw InvalidActorException.InvalidActorFactoryType(actorType, factoryType);

    if (factoryType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Length != 0)
      throw InvalidActorException.InvalidActorFactoryType(actorType, factoryType);

    if (factoryType.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Length != 0)
      throw InvalidActorException.InvalidActorFactoryType(actorType, factoryType);

    MethodInfo? createAsyncMethod;

    if (!state.IsVirtual)
    {
      Type[] createAsyncMethodTypes = (id.HasIdSource ? Type.EmptyTypes : new[] { typeof(string) })
        .Concat(state.Fields.Select(field => field.FieldType))
        .Concat(new[] { typeof(CancellationToken) })
        .ToArray();

      createAsyncMethod = factoryType.GetMethod(
        name: "CreateAsync",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        binder: null,
        types: createAsyncMethodTypes,
        modifiers: null);

      if (createAsyncMethod is null || createAsyncMethod.ReturnType != typeof(Task<>).MakeGenericType(interfaceType))
        throw InvalidActorException.InvalidActorFactoryType(actorType, factoryType);
    }
    else
    {
      createAsyncMethod = null;
    }

    MethodInfo? getMethod = factoryType.GetMethod(
      name: "Get",
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      binder: null,
      types: new[] { id.IdType },
      modifiers: null);

    if (getMethod is null || getMethod.ReturnType != interfaceType)
      throw InvalidActorException.InvalidActorFactoryType(actorType, factoryType);

    return new ActorFactoryDescriptor(factoryType, createAsyncMethod, getMethod);
  }
}
