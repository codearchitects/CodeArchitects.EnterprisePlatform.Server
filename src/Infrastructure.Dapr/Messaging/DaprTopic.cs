using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

/// <summary>
/// Utilities for Dapr topic names.
/// </summary>
internal static class DaprTopic
{
  /// <summary>
  /// The default topic name.
  /// </summary>
  public const string Default = "__global";

  /// <summary>
  /// Creates the complete Dapr topic name using the topic name and the message type.
  /// </summary>
  /// <param name="topic">The topic name.</param>
  /// <param name="messageType">The message type.</param>
  /// <returns>The complete Dapr topic.</returns>
  public static string Make(string? topic, Type messageType) => $"{topic ?? Default}-{messageType.Name}";
}
