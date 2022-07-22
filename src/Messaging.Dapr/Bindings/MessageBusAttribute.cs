namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)]
public class MessageBusAttribute : Attribute, IMessageBusOutputMetadata
{
  public MessageBusAttribute(string bus)
  {
    Bus = bus;
  }

  public MessageBusAttribute(string bus, string? topic)
  {
    Bus = bus;
    Topic = topic;
  }

  public MessageBusAttribute(string bus, params Type[] resultTypes)
  {
    Bus = bus;
    AllowedTypes = resultTypes;
  }

  public MessageBusAttribute(string bus, string? topic, params Type[] resultTypes)
  {
    Bus = bus;
    Topic = topic;
    AllowedTypes = resultTypes;
  }

  public string Bus { get; }

  public string? Topic { get; init; }

  public IEnumerable<Type>? AllowedTypes { get; }
}
