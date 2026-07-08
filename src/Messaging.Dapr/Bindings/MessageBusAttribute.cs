namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

/// <summary>
/// Indicates that the result of a handler method should be published to a message bus.
/// </summary>
[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)]
public class MessageBusAttribute : Attribute, IMessageBusOutputMetadata
{
  /// <summary>
  /// Creates a new <see cref="MessageBusAttribute"/>.
  /// </summary>
  public MessageBusAttribute()
  {
  }

  /// <summary>
  /// Creates a new <see cref="MessageBusAttribute"/>, specifying the set of result types that will be subject to the output binding.
  /// </summary>
  /// <param name="resultTypes">The set of result types that will be subject to the output binding.</param>
  public MessageBusAttribute(params Type[] resultTypes)
  {
    AllowedTypes = resultTypes;
  }

  /// <summary>
  /// Creates a new <see cref="MessageBusAttribute"/>, specifying the bus name and the topic name.
  /// </summary>
  /// <param name="bus">The name of the bus to publish the result to.</param>
  /// <param name="topic">The name of the topic to publish the result to.</param>
  public MessageBusAttribute(string bus, string topic)
  {
    Bus = bus;
    Topic = topic;
  }

  /// <summary>
  /// Creates a new <see cref="MessageBusAttribute"/>, specifying the bus name, the topic name and the set of result types that will be subject to the output binding.
  /// </summary>
  /// <param name="bus">The name of the bus to publish the result to.</param>
  /// <param name="topic">The name of the topic to publish the result to.</param>
  /// <param name="resultTypes">The set of result types that will be subject to the output binding.</param>
  public MessageBusAttribute(string bus, string topic, params Type[] resultTypes)
  {
    Bus = bus;
    Topic = topic;
    AllowedTypes = resultTypes;
  }

  /// <summary>
  /// The name of the bus to publish the result to.
  /// </summary>
  public string? Bus { get; init; }

  /// <summary>
  /// The name of the topic to publish the result to.
  /// </summary>
  public string? Topic { get; init; }

  /// <summary>
  /// The set of result types that will be subject to the output binding.
  /// </summary>
  public IEnumerable<Type>? AllowedTypes { get; }
}
