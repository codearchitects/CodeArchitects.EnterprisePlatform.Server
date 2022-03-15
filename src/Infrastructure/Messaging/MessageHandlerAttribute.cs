using System;

namespace CodeArchitects.Platform.Infrastructure.Messaging;

/// <summary>
/// Specifies the class or a method is a message handler and will be triggered by messages.
/// An optional bus name can be indicated, otherwise the default configured bus name will be used.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)] // TODO: Allow multiple, so a single class/method can handle multiple busses and topics
public class MessageHandlerAttribute : Attribute
{
  /// <summary>
  /// Initializes a new instance of <see cref="MessageHandlerAttribute"/> without specifing a bus name.
  /// </summary>
  public MessageHandlerAttribute()
  {
  }

  /// <summary>
  /// Initializes a new instance of <see cref="MessageHandlerAttribute"/> specifying a bus name.
  /// </summary>
  /// <param name="busName">The name of the bus.</param>
  public MessageHandlerAttribute(string busName)
  {
    BusName = busName;
  }

  /// <summary>
  /// Initializes a new instance of <see cref="MessageHandlerAttribute"/> specifying a bus name.
  /// </summary>
  /// <param name="busName">The name of the bus.</param>
  /// <param name="busName">The name of the topic.</param>
  public MessageHandlerAttribute(string busName, string topic)
  {
    BusName = busName;
    Topic = topic;
  }

  /// <summary>
  /// The name of the bus.
  /// </summary>
  public string? BusName { get; init; }

  /// <summary>
  /// The name of the topic.
  /// </summary>
  public string? Topic { get; init; }
}
