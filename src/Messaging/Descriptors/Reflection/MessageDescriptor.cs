using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Reflection;

internal record MessageDescriptor(
  Type Type,
  string Name) : IMessageDescriptor
{
  public static MessageDescriptor Create(Type messageType)
  {
    MessageAttribute? attribute = messageType.GetCustomAttribute<MessageAttribute>();
    string name = attribute?.MessageName ?? messageType.Name;

    return new MessageDescriptor(messageType, name);
  }
}
