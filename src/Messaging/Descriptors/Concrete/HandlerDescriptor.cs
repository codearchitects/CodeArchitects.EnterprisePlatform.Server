namespace CodeArchitects.Platform.Messaging.Descriptors.Concrete;

internal record HandlerDescriptor(
  Type ConcreteType,
  IReadOnlyCollection<IHandlerIdentityDescriptor> Identities) : IHandlerDescriptor
{
  public static HandlerDescriptor Create(string? defaultBus, string defaultTopic, Type concreteType)
  {
    IReadOnlyCollection<IHandlerIdentityDescriptor> identities = HandlerIdentityDescriptor.Create(defaultBus, defaultTopic, concreteType);

    return new HandlerDescriptor(concreteType, identities);
  }
}