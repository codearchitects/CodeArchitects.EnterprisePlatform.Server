namespace CodeArchitects.Platform.Messaging.Descriptors.Reflection;

internal record MessagingDescriptor(
  IEnumerable<IHandlerDescriptor> HandlerDescriptors,
  IEnumerable<IMessageDescriptor> MessageDescriptors,
  IReadOnlyCollection<HandlerDiagnostics> Diagnostics) : IMessagingDescriptor
{
  public static MessagingDescriptor Create(IEnumerable<Type> concreteTypes, string? defaultBus, string? defaultTopic)
  {
    List<IHandlerDescriptor> handlerDescriptors = new();
    Dictionary<Type, IMessageDescriptor> messageDescriptors = new();
    List<HandlerDiagnostics> diagnostics = new();
    
    foreach (Type concreteType in concreteTypes)
    {
      foreach (HandlerDescriptor handlerDescriptor in HandlerDescriptor.Create(concreteType, defaultBus, defaultTopic, diagnostics))
      {
        handlerDescriptors.Add(handlerDescriptor);
        
        Type messageType = handlerDescriptor.MessageType;
        if (!messageDescriptors.ContainsKey(messageType))
        {
          messageDescriptors.Add(messageType, MessageDescriptor.Create(messageType));
        }
      }
    }

    return new MessagingDescriptor(handlerDescriptors, messageDescriptors.Values, diagnostics);
  }
}
