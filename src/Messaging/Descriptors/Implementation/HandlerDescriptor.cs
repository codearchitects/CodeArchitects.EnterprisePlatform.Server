using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

/// <summary>
/// Implementation of <see cref="IHandlerDescriptor"/>
/// </summary>
internal record HandlerDescriptor(
  string Bus,
  string Topic,
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

  public static IEnumerable<HandlerDescriptor> Create(Type concreteType, string? defaultBus, string? defaultTopic, ICollection<HandlerDiagnostics> diagnostics)
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
        diagnostics.Add(HandlerDiagnostics.MultipleMessageHandlerAttributeOnClass(concreteType));
      }
    }

    IEnumerable<Type> handlerInterfaceTypes = concreteType
      .GetInterfaces()
      .Where(IsMessageHandlerInterface);

    foreach (Type handlerInterfaceType in handlerInterfaceTypes)
    {
      InterfaceMapping mapping = concreteType.GetInterfaceMap(handlerInterfaceType);
      MethodInfo handlerMethod = mapping.TargetMethods[0];

      IReadOnlyCollection<IOutputBindingDescriptor> outputBindings = OutputBindingDescriptor.Create(handlerMethod);

      IEnumerable<MessageHandlerAttribute> methodAttributes = handlerMethod.GetCustomAttributes<MessageHandlerAttribute>();
      if (!methodAttributes.Any())
      {
        methodAttributes = new[] { new MessageHandlerAttribute() };
      }

      foreach (MessageHandlerAttribute methodAttribute in methodAttributes)
      {
        string? bus = methodAttribute.Bus ?? defaultBus;
        string? topic = methodAttribute.Topic ?? defaultTopic;

        if (bus is not null && topic is not null)
        {
          yield return new HandlerDescriptor(bus, topic, handlerInterfaceType, concreteType, OutputBindingDescriptor.Create(handlerMethod));
        }
        else
        {
          if (bus is null)
            diagnostics.Add(HandlerDiagnostics.NullBusOnHandler(concreteType, handlerMethod));
          if (topic is null)
            diagnostics.Add(HandlerDiagnostics.NullTopicOnHandler(concreteType, handlerMethod));
        }
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
}