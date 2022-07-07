using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Messaging.AspNetCore.Configuration;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

internal class DaprMessagingOptionsBuilder : IDaprMessagingOptionsBuilder
{
  private readonly MessagingConfig _config;
  private readonly HashSet<Type> _handlerTypes;
  private readonly HashSet<Type> _messageTypes;

  public DaprMessagingOptionsBuilder(MessagingConfig config)
  {
    _handlerTypes = new();
    _messageTypes = new();
    _config = config;
  }

  public IEnumerable<Type> HandlerTypes => _handlerTypes;

  public IEnumerable<Type> MessageTypes => _messageTypes;

  public IDaprMessagingOptionsBuilder Configure(Action<MessagingConfig> configure)
  {
    configure(_config);
    return this;
  }

  public IDaprMessagingOptionsBuilder AddHandler(Type handlerType)
  {
    if (!handlerType.ImplementsGenericInterface(typeof(IMessageHandler<>)) && !handlerType.ImplementsGenericInterface(typeof(IMessageHandler<,>)))
      throw new ArgumentException($"'{nameof(handlerType)}' must be a type implementing the IMessageHandler interface", nameof(handlerType));

    _handlerTypes.Add(handlerType);

    return this;
  }

  public IDaprMessagingOptionsBuilder AddMessage(Type messageType)
  {
    _messageTypes.Add(messageType);
    return this;
  }

  public IDaprMessagingOptionsBuilder ScanAssembly(Assembly assembly)
  {
    foreach (Type type in assembly.GetTypes())
    {
      if (type.IsDefined(typeof(MessageHandlerAttribute)))
      {
        AddHandler(type);
        continue;
      }

      if (type.IsDefined(typeof(MessageAttribute)))
      {
        AddMessage(type);
        continue;
      }
    }

    return this;
  }
}
