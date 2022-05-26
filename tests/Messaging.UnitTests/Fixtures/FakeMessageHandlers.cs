using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Fixtures;

[MessageHandler(StandardMessageHandlerInfo.ClassBus, StandardMessageHandlerInfo.ClassTopic)]
public class StandardMessageHandler : IMessageHandler<Message1>, IMessageHandler<Message2, Message1>
{
  [MessageHandler(StandardMessageHandlerInfo.Identity1Bus, StandardMessageHandlerInfo.Identity1Topic)]
  public Task HandleAsync(Message1 message, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  [return: FakeOutputBinding1("x")]
  public Task<Message1> HandleAsync(Message2 message, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}

public static class StandardMessageHandlerInfo
{
  public const string ClassBus = "bus";
  public const string ClassTopic = "topic";
  public const string Identity1Bus = "override_bus";
  public const string Identity1Topic = "override_topic";
  public static readonly FakeOutputBinding1Attribute Identity2MetadataObject;

  static StandardMessageHandlerInfo()
  {
    MethodInfo identity2Method = typeof(StandardMessageHandler).GetMethod(
      name: nameof(StandardMessageHandler.HandleAsync),
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { typeof(Message2), typeof(CancellationToken) }) ?? throw new MissingMethodException(typeof(StandardMessageHandler).Name, nameof(StandardMessageHandler.HandleAsync));

    Identity2MetadataObject = (FakeOutputBinding1Attribute)identity2Method.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(FakeOutputBinding1Attribute), false)[0];
  }
}