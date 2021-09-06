using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging
{
  /// <summary>
  /// Represents the identity of an handler, that is characterized by the name of the bus and the topic it is subscribed to, along with the type of the message it handles.
  /// </summary>
  internal readonly struct MessageHandlerIdentity : IEquatable<MessageHandlerIdentity>
  {
    /// <summary>
    /// Creates an instance of <see cref="MessageHandlerIdentity"/>.
    /// </summary>
    /// <param name="busName">The name of the bus the handler is subscribed to.</param>
    /// <param name="topic">The name of the topic the handler is subscribed to.</param>
    /// <param name="messageType">The type of the message the handler handles.</param>
    public MessageHandlerIdentity(string? busName, string? topic, Type messageType)
    {
      BusName = busName;
      Topic = topic;
      MessageType = messageType;
    }

    /// <summary>
    /// The name of the bus the handler is subscribed to.
    /// </summary>
    public readonly string? BusName { get; }

    /// <summary>
    /// The name of the topic the handler is subscribed to.
    /// </summary>
    public readonly string? Topic { get; }

    /// <summary>
    /// The type of the message the handler handles.
    /// </summary>
    public readonly Type MessageType { get; }

    /// <summary>
    /// The topic that Dapr will use to refer to this identity.
    /// </summary>
    public readonly string DaprTopic => Messaging.DaprTopic.Make(Topic, MessageType);

    public readonly bool Equals(MessageHandlerIdentity other)
      => BusName == other.BusName
      && Topic == other.Topic
      && MessageType == other.MessageType;

    public readonly override bool Equals(object? obj)
      => obj is MessageHandlerIdentity other && Equals(other);

    public readonly override int GetHashCode()
      => HashCode.Combine(BusName, Topic, MessageType);

    /// <summary>
    /// Indicates whether the <see cref="MessageHandlerIdentity"/> instance on the left-hand side is equal to the <see cref="MessageHandlerIdentity"/> instance on the right-hand side.
    /// </summary>
    /// <param name="left">The left-hand side <see cref="MessageHandlerIdentity"/>.</param>
    /// <param name="right">The right-hand side <see cref="MessageHandlerIdentity"/>.</param>
    /// <returns>True if the arguments are equal, false otherwise.</returns>
    public static bool operator ==(MessageHandlerIdentity left, MessageHandlerIdentity right)
      => left.Equals(right);

    /// <summary>
    /// Indicates whether the <see cref="MessageHandlerIdentity"/> instance on the left-hand side is not equal to the <see cref="MessageHandlerIdentity"/> instance on the right-hand side.
    /// </summary>
    /// <param name="left">The left-hand side <see cref="MessageHandlerIdentity"/>.</param>
    /// <param name="right">The right-hand side <see cref="MessageHandlerIdentity"/>.</param>
    /// <returns>True if the arguments are not equal, false otherwise.</returns>
    public static bool operator !=(MessageHandlerIdentity left, MessageHandlerIdentity right)
      => !(left == right);
  }
}
