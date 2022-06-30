namespace CodeArchitects.Platform.Messaging.Descriptors.Reflection;

/// <summary>
/// Implementation of <see cref="IMessagingDescriptor"/>
/// </summary>
internal record MessagingDescriptor(
  IEnumerable<IHandlerDescriptor> HandlerDescriptors,
  IEnumerable<IMessageDescriptor> MessageDescriptors,
  IReadOnlyCollection<HandlerDiagnostics> Diagnostics) : IMessagingDescriptor
{
  public static MessagingDescriptor Create(IEnumerable<Type> concreteTypes, IEnumerable<Type> messageTypes, string? defaultBus, string? defaultTopic)
  {
    List<IHandlerDescriptor> handlerDescriptors = new();
    Dictionary<Type, IMessageDescriptor> messageDescriptors = new();
    List<HandlerDiagnostics> diagnostics = new();

    foreach (Type messageType in messageTypes)
    {
      TryAddMessageDescriptor(messageType);
    }

    foreach (Type concreteType in concreteTypes)
    {
      foreach (HandlerDescriptor handlerDescriptor in HandlerDescriptor.Create(concreteType, defaultBus, defaultTopic, diagnostics))
      {
        handlerDescriptors.Add(handlerDescriptor);
        TryAddMessageDescriptor(handlerDescriptor.MessageType);
      }
    }

    return new MessagingDescriptor(handlerDescriptors, messageDescriptors.Values, diagnostics);

    void TryAddMessageDescriptor(Type messageType)
    {
      while (messageType != typeof(object))
      {
        if (!messageDescriptors.ContainsKey(messageType))
        {
          messageDescriptors.Add(messageType, MessageDescriptor.Create(messageType));
        }

        messageType = messageType.BaseType!;
      }
    }
  }
}
