using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

/// <summary>
/// Indicates that a handler is subscribed to a given topic.
/// </summary>
/// <remarks>
/// If not applied, the name of the message class is used as the topic name.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class SubscribeToTopicAttribute : Attribute
{
  /// <summary>
  /// Creates a new <see cref="SubscribeToTopicAttribute"/> instance.
  /// </summary>
  /// <param name="topic">The name of the topic.</param>
  public SubscribeToTopicAttribute(string topic)
  {
    if (string.IsNullOrWhiteSpace(topic))
      throw new ArgumentException($"'{nameof(topic)}' cannot be null or whitespace.", nameof(topic));

    Topic = topic;
  }

  /// <summary>
  /// The name of the topic.
  /// </summary>
  public string Topic { get; }
}
