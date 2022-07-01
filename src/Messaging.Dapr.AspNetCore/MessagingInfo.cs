using CodeArchitects.Platform.Messaging.AspNetCore.Utils;
using CodeArchitects.Platform.Messaging.Dapr.AspNetCore.Configuration;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

internal class MessagingInfo : IMessagingInfo
{
  private readonly IMessageBiMap _messageMap;
  private readonly MessagingConfig _config;

  public MessagingInfo(IMessageBiMap messageMap, MessagingConfig config)
  {
    _messageMap = messageMap;
    _config = config;
  }

  public string GetMessageName(Type messageType)
  {
    if (_messageMap.TryGetValue(messageType, out string? name))
      return name;

    return messageType.Name;
  }

  public bool IsBusKnown(string busName)
  {
    return _config.BusNames?.Contains(busName) ?? true;
  }
}
