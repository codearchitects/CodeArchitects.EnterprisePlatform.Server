namespace CodeArchitects.Platform.Messaging.Descriptors.Reflection;

internal record MessagingDescriptor(
  IEnumerable<IHandlerDescriptor> HandlerDescriptors,
  IReadOnlyCollection<HandlerDiagnostics> Diagnostics) : IMessagingDescriptor
{
  public static MessagingDescriptor Create(IEnumerable<Type> concreteTypes, string? defaultBus, string? defaultTopic)
  {
    List<IHandlerDescriptor> handlerDescriptors = new();
    List<HandlerDiagnostics> diagnostics = new();
    
    foreach (Type concreteType in concreteTypes)
    {
      handlerDescriptors.AddRange(HandlerDescriptor.Create(concreteType, defaultBus, defaultTopic, diagnostics));
    }

    return new MessagingDescriptor(handlerDescriptors, diagnostics);
  }
}
