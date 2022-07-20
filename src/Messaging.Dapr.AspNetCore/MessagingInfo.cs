using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using CodeArchitects.Platform.Messaging.AspNetCore.Utils;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

internal class MessagingInfo : IMessagingInfo
{
  private readonly IMessageBiMap _messageMap;
  private readonly HashSet<string> _busNames;
  private readonly string? _defaultBus;

  public MessagingInfo(IMessageBiMap messageMap, HashSet<string> busNames, string? defaultBus)
  {
    _messageMap = messageMap;
    _busNames = busNames;
    _defaultBus = defaultBus;
  }

  public string GetMessageName(Type messageType)
  {
    if (_messageMap.TryGetValue(messageType, out string? name))
      return name;

    return messageType.Name;
  }

  public bool IsBusKnown(string busName)
  {
    if (_defaultBus is not null && _defaultBus == busName)
      return true;

    if (_busNames is null)
      return true;

    return _busNames.Contains(busName);
  }

  public string? GetDefaultBus()
  {
    return _defaultBus is not null
      ? _defaultBus
      : _busNames.Count is 1
        ? _busNames.First()
        : null;
  }

  public static MessagingInfo Create(IMessageBiMap messageMap, IDaprComponentAccessor componentAccessor, string? defaultBus)
  {
    HashSet<string> busNames = new();
    foreach (string busName in componentAccessor.Components.GetComponentNames("pubsub"))
    {
      busNames.Add(busName);
    }

    return new MessagingInfo(messageMap, busNames, defaultBus);
  }
}
