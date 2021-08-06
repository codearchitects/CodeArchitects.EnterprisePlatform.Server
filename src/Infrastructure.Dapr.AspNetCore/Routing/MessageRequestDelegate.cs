using CodeArchitects.Platform.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Routing
{
  /// <summary>
  /// Provides a context for an action that is mapped to a pubsub endpoint.
  /// </summary>
  internal abstract class MessageRequestDelegate
  {
    /// <summary>
    /// Creates an instance of <see cref="MessageRequestDelegate"/>.
    /// </summary>
    /// <param name="handlerImplementationType">The type of the handler that will handle the received message.</param>
    /// <param name="serializerOptions">The serializer options.</param>
    public MessageRequestDelegate(Type handlerImplementationType, JsonSerializerOptions serializerOptions)
    {
      if (!handlerImplementationType.IsAssignableTo(HandlerType))
        throw new InvalidHandlerConfigurationException(HandlerType, handlerImplementationType);

      SerializerOptions = serializerOptions;
      HandlerImplementationType = handlerImplementationType;
    }

    /// <summary>
    /// The type of the handlers that will handle the received message.
    /// </summary>
    protected Type HandlerImplementationType { get; }

    /// <summary>
    /// The serializer options.
    /// </summary>
    protected JsonSerializerOptions SerializerOptions { get; }

    /// <summary>
    /// Action that executes when the message endpoint is matched.
    /// </summary>
    /// <param name="context">The HTTP context information.</param>
    /// <returns>A task that completes when all handlers complete.</returns>
    public abstract Task ExecuteAsync(HttpContext context);

    /// <summary>
    /// The specific <see cref="IMessageHandler{TMessage}"/> type.
    /// </summary>
    protected abstract Type HandlerType { get; }
  }
}
