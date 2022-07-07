using CodeArchitects.Platform.Messaging.AspNetCore.Configuration;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

public interface IDaprMessagingOptionsBuilder
{
  IDaprMessagingOptionsBuilder Configure(Action<MessagingConfig> configure);
  IDaprMessagingOptionsBuilder AddHandler(Type handlerType);
  IDaprMessagingOptionsBuilder AddMessage(Type messageType);
  IDaprMessagingOptionsBuilder ScanAssembly(Assembly assembly);
}
