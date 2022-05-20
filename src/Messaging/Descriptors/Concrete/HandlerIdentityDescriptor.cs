using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Concrete;

internal record HandlerIdentityDescriptor(
  Type InterfaceType,
  IReadOnlyCollection<IOutputBindingDescriptor> OutputBindings,
  string Bus,
  string Topic) : IHandlerIdentityDescriptor
{
  public Type MessageType => InterfaceType.GetGenericArguments()[0];

  public Type? ResultType
  {
    get
    {
      Type[] genericArguments = InterfaceType.GetGenericArguments();
      return genericArguments.Length == 2
        ? genericArguments[1]
        : null;
    }
  }

  public static IReadOnlyCollection<HandlerIdentityDescriptor> Create(string? defaultBus, string defaultTopic, Type concreteType)
  {
    IEnumerable<MessageHandlerAttribute> classAttributes = concreteType.GetCustomAttributes<MessageHandlerAttribute>();

    List<HandlerIdentityDescriptor> descriptors = new List<HandlerIdentityDescriptor>();

    IEnumerable<Type> handlerInterfaceTypes = concreteType
      .GetInterfaces()
      .Where(IsMessageHandlerInterface);

    foreach (MessageHandlerAttribute classAttribute in classAttributes)
    {
      foreach (Type interfaceType in handlerInterfaceTypes)
      {
        InterfaceMapping mapping = concreteType.GetInterfaceMap(interfaceType);
        MethodInfo handlerMethod = mapping.TargetMethods[0];

        IReadOnlyCollection<IOutputBindingDescriptor> outputBindings = OutputBindingDescriptor.Create(handlerMethod);

        IEnumerable<MessageHandlerAttribute> methodAttributes = handlerMethod.GetCustomAttributes<MessageHandlerAttribute>();
        foreach (MessageHandlerAttribute methodAttribute in methodAttributes)
        {
          string? bus = methodAttribute.Bus ?? classAttribute.Bus ?? defaultBus;
          if (bus is null)
            continue; // TODO: Log warning
          string topic = methodAttribute.Topic ?? classAttribute.Topic ?? defaultTopic;

          HandlerIdentityDescriptor descriptor = new HandlerIdentityDescriptor(interfaceType, outputBindings, bus, topic);
          descriptors.Add(descriptor);
        }
      }
    }

    return descriptors;
  }

  private static bool IsMessageHandlerInterface(Type interfaceType)
  {
    if (!interfaceType.IsGenericType)
      return false;

    Type interfaceTypeDefinition = interfaceType.GetGenericTypeDefinition();

    return interfaceTypeDefinition == typeof(IMessageHandler<>) || interfaceTypeDefinition == typeof(IMessageHandler<,>);
  }
}
