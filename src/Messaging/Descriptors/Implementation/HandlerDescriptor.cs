using OneOf;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

/// <summary>
/// Implementation of <see cref="IHandlerDescriptor"/>
/// </summary>
internal record HandlerDescriptor(
  string? Bus,
  string? Topic,
  Type MessageType,
  Type ResultType,
  Type ConcreteType,
  IReadOnlyCollection<IOutputBindingDescriptor> OutputBindingDescriptors) : IHandlerDescriptor
{
  public Type InterfaceType => HasResult ? typeof(IMessageHandler<,>).MakeGenericType(MessageType, ResultType) : typeof(IMessageHandler<>).MakeGenericType(MessageType);

  public bool HasResult => ResultType != typeof(void);

  public bool HasUnionResult => typeof(IOneOf).IsAssignableFrom(ResultType);

  public IReadOnlyList<Type> ResultTypes => HasUnionResult ? ResultType.GetGenericArguments() : Array.Empty<Type>();

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

      IReadOnlyCollection<IOutputBindingDescriptor> outputBindings = OutputBindingDescriptor.Create(handlerMethod);

      IEnumerable<MessageHandlerAttribute> methodAttributes = handlerMethod.GetCustomAttributes<MessageHandlerAttribute>();
      if (!methodAttributes.Any())
      {
        methodAttributes = new[] { new MessageHandlerAttribute() };
      }

      foreach (MessageHandlerAttribute methodAttribute in methodAttributes)
      {
        Type[] interfaceGenericArguments = handlerInterfaceType.GetGenericArguments();
        Type messageType = interfaceGenericArguments[0];
        Type resultType = interfaceGenericArguments.Length == 1 ? typeof(void) : interfaceGenericArguments[1];

        yield return new HandlerDescriptor(
          Bus: methodAttribute.Bus ?? defaultBus,
          Topic: methodAttribute.Topic ?? defaultTopic,
          MessageType: messageType,
          ResultType: resultType,
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