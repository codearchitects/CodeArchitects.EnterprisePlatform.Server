using System;
using System.Collections.Generic;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

/// <summary>
/// The configuration of the registered handlers.
/// </summary>
internal interface IMessageHandlerConfiguration
{
  /// <summary>
  /// Represents an association between a <see cref="MessageHandlerIdentity"/> and the handler type that corresponds to that identity,
  /// that is, the type that expose a handling method on the same message, subscribed to the same bus and topic.
  /// </summary>
  IReadOnlyDictionary<MessageHandlerIdentity, Type> HandlerMap { get; }
}
