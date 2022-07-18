using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

/// <summary>
/// Implementation of <see cref="IHandlerDescriptor"/>
/// </summary>
internal record HandlerDescriptor(
  string? Bus,
  string? Topic,
  Type InterfaceType,
  Type ConcreteType,
  IReadOnlyCollection<IOutputBindingDescriptor> OutputBindingDescriptors) : IHandlerDescriptor
{
  public Type MessageType => InterfaceType.GetGenericArguments()[0];

  public Type ResultType
  {
    get
    {
      Type[] genericArguments = InterfaceType.GetGenericArguments();
      return genericArguments.Length == 2
        ? genericArguments[1]
        : typeof(void);
    }
  }

  public static IEnumerable<HandlerDescriptor> Create(Type concreteType, string? defaultBus, string? defaultTopic, ICollection<HandlerDiagnostics> diagnosticCollection)
  {
    IEnumerable<MessageHandlerAttribute> classAttributes = concreteType.GetCustomAttributes<MessageHandlerAttribute>();
    int classAttributesCount = classAttributes.Count();
    if (classAttributesCount > 0)
    {
      MessageHandlerAttribute classAttribute = classAttributes.First();
      if (classAttribute.Bus is not null)
        defaultBus = classAttribute.Bus;
      if (classAttribute.Topic is not null)
        defaultTopic = classAttribute.Topic;

      if (classAttributesCount > 1)
      {
        diagnosticCollection.Add(MultipleMessageHandlerAttributeOnClass(concreteType));
      }
    }

    IEnumerable<Type> handlerInterfaceTypes = concreteType
      .GetInterfaces()
      .Where(IsMessageHandlerInterface);

    foreach (Type handlerInterfaceType in handlerInterfaceTypes)
    {
      InterfaceMapping mapping = concreteType.GetInterfaceMap(handlerInterfaceType);
      MethodInfo handlerMethod = mapping.TargetMethods[0];
      if (handlerMethod.IsDefined(typeof(DoesNotHandleAttribute)))
        continue;

      IReadOnlyCollection<IOutputBindingDescriptor> outputBindings = OutputBindingDescriptor.Create(handlerMethod, diagnosticCollection);

      IEnumerable<MessageHandlerAttribute> methodAttributes = handlerMethod.GetCustomAttributes<MessageHandlerAttribute>();
      if (!methodAttributes.Any())
      {
        methodAttributes = new[] { new MessageHandlerAttribute() };
      }

      foreach (MessageHandlerAttribute methodAttribute in methodAttributes)
      {
        yield return new HandlerDescriptor(
          Bus: methodAttribute.Bus ?? defaultBus,
          Topic: methodAttribute.Topic ?? defaultTopic,
          InterfaceType: handlerInterfaceType,
          ConcreteType: concreteType,
          OutputBindingDescriptors: outputBindings);
      }
    }

    static bool IsMessageHandlerInterface(Type interfaceType)
    {
      if (!interfaceType.IsGenericType)
        return false;

      Type interfaceTypeDefinition = interfaceType.GetGenericTypeDefinition();

      return interfaceTypeDefinition == typeof(IMessageHandler<>) || interfaceTypeDefinition == typeof(IMessageHandler<,>);
    }
  }

  private static HandlerDiagnostics MultipleMessageHandlerAttributeOnClass(Type concreteType)
    => new(concreteType, $"Multiple {nameof(MessageHandlerAttribute)} found on type {{0}}", concreteType);
}