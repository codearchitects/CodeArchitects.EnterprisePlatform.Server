using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Concrete;

internal record HandlerIdentityDescriptor(
  Type InterfaceType,
  IReadOnlyCollection<IOutputBindingDescriptor> OutputBindingDescriptors,
  string? Bus,
  string? Topic) : IHandlerIdentityDescriptor
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

  public static IReadOnlyCollection<HandlerIdentityDescriptor> Create(string? defaultBus, string? defaultTopic, Type concreteType)
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
        if (methodAttributes.Any())
        {
          foreach (MessageHandlerAttribute methodAttribute in methodAttributes)
          {
            AddIdentityDescriptor(methodAttribute.Bus, methodAttribute.Topic);
          }
        }
        else
        {
          AddIdentityDescriptor(null, null);
        }

        void AddIdentityDescriptor(string? bus, string? topic)
        {
          bus ??= classAttribute.Bus ?? defaultBus;
          topic ??= classAttribute.Topic ?? defaultTopic;

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
