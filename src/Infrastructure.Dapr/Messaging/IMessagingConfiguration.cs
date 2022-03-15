using CodeArchitects.Platform.Common.Internals;
using CodeArchitects.Platform.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

/// <summary>
/// Exposes information about messaging.
/// </summary>
internal interface IMessagingConfiguration
{
  /// <summary>
  /// Represents an association between a <see cref="MessageHandlerIdentity"/> and an <see cref="ImplementationPair"/> that corresponds to that identity:
  /// the <see cref="ImplementationPair.ImplementationType"/> of the pair exposes a handling method for the same <see cref="MessageHandlerIdentity.BusName"/>, <see cref="MessageHandlerIdentity.Topic"/> and <see cref="MessageHandlerIdentity.MessageType"/>.
  /// </summary>
  IReadOnlyDictionary<MessageHandlerIdentity, ImplementationPair> HandlerMap { get; }

  /// <summary>
  /// Collection of all registered message types.
  /// </summary>
  /// <remarks>
  /// This collection will include all message types decorated with the <see cref="MessageAttribute"/> and/or type arguments of all registered <see cref="IMessageHandler{TMessage}"/> interfaces.
  /// </remarks>
  ICollection<Type> MessageTypes { get; } // IReadOnlyCollection does not expose a 'Contains' method but this should be immutable
}
