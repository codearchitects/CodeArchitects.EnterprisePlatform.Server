using CodeArchitects.Platform.Infrastructure.Messaging;
using System;
using System.Reflection;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

internal class MessageEnvelope<TMessage>
  where TMessage : class
{
  public MessageEnvelope(TMessage message)
  {
    Message = message;
  }

  public TMessage Message { get; }

  public string Type
  {
    get
    {
      Type messageType = typeof(TMessage);
      return messageType.GetCustomAttribute<MessageAttribute>()?.MessageName ?? messageType.Name;
    }
  }
}

internal static class MessageEnvelope
{
  public static MessageEnvelope<TMessage> Create<TMessage>(TMessage message) where TMessage : class
    => new MessageEnvelope<TMessage>(message);
}