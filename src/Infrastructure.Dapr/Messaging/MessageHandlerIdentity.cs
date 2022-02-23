using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

/// <summary>
/// Represents the identity of an handler, that is characterized by the name of the bus and the topic it is subscribed to, along with the type of the message it handles.
/// </summary>
/// <param name="BusName">The name of the bus the handler is subscribed to.</param>
/// <param name="Topic">The name of the topic the handler is subscribed to.</param>
/// <param name="MessageType">The type of the message the handler handles.</param>
internal readonly record struct MessageHandlerIdentity(string? BusName, string? Topic, Type MessageType)
{
  /// <summary>
  /// The topic that Dapr will use to refer to this identity.
  /// </summary>
  public readonly string DaprTopic => Messaging.DaprTopic.Make(Topic, MessageType);
}
