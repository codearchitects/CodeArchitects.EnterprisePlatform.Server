namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)]
public class MessageBusAttribute : Attribute, IMessageBusOutputMetadata
{
  public MessageBusAttribute(params Type[] resultTypes)
  {
    AllowedTypes = resultTypes;
  }

  public MessageBusAttribute(string bus, string topic, params Type[] resultTypes)
  {
    Bus = bus;
    Topic = topic;
    AllowedTypes = resultTypes;
  }

  public string? Bus { get; init; }

  public string? Topic { get; init; }

  public IEnumerable<Type>? AllowedTypes { get; }
}
