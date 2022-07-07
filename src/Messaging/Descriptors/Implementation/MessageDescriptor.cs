using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

/// <summary>
/// Implementation of <see cref="IMessageDescriptor"/>
/// </summary>
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
