namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

[AttributeUsage(AttributeTargets.ReturnValue)]
public class MessageBusAttribute : Attribute, IMessageBusOutputMetadata
{
  public MessageBusAttribute(string busName)
  {
    BusName = busName;
  }

  public MessageBusAttribute(string busName, string? topic)
  {
    BusName = busName;
    Topic = topic;
  }

  public string BusName { get; }

  public string? Topic { get; init; }
}
