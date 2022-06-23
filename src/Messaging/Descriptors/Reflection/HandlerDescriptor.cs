namespace CodeArchitects.Platform.Messaging.Descriptors.Reflection;

internal record HandlerDescriptor(
  Type ConcreteType,
  IReadOnlyCollection<IHandlerIdentityDescriptor> IdentityDescriptors) : IHandlerDescriptor
{
  public static HandlerDescriptor Create(string? defaultBus, string? defaultTopic, Type concreteType)
  {
    IReadOnlyCollection<IHandlerIdentityDescriptor> identities = HandlerIdentityDescriptor.Create(defaultBus, defaultTopic, concreteType);

    return new HandlerDescriptor(concreteType, identities);
  }
}