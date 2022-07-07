namespace CodeArchitects.Platform.Messaging;

/// <summary>
/// Specifies the class or a method is a message handler and will be triggered by messages.
/// An optional bus name can be indicated, otherwise the default configured bus name will be used.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class MessageHandlerAttribute : Attribute
{
  /// <summary>
  /// Initializes a new <see cref="MessageHandlerAttribute"/> instance without specifing a bus name.
  /// </summary>
  public MessageHandlerAttribute()
  {
  }

  /// <summary>
  /// Initializes a new <see cref="MessageHandlerAttribute"/> instance specifying a bus name.
  /// </summary>
  /// <param name="bus">The name of the bus.</param>
  public MessageHandlerAttribute(string bus)
  {
    Bus = bus;
  }

  /// <summary>
  /// Initializes a new <see cref="MessageHandlerAttribute"/> instance specifying a bus name.
  /// </summary>
  /// <param name="bus">The name of the bus.</param>
  /// <param name="topic">The name of the topic.</param>
  public MessageHandlerAttribute(string bus, string topic)
  {
    Bus = bus;
    Topic = topic;
  }

  /// <summary>
  /// The name of the bus.
  /// </summary>
  public string? Bus { get; init; }

  /// <summary>
  /// The name of the topic.
  /// </summary>
  public string? Topic { get; init; }
}
