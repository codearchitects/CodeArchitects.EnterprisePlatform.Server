using CodeArchitects.Platform.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Routing
{
  /// <summary>
  /// Provides a context for an action that is mapped to a pubsub endpoint and routes instances of <typeparamref name="TMessage"/> to the configured handlers.
  /// </summary>
  /// <remarks>
  /// Inherits from <see cref="MessageRequestDelegate"/>.
  /// </remarks>
  /// <typeparam name="TMessage">The type of the message that is handled.</typeparam>
  internal sealed class MessageRequestDelegate<TMessage> : MessageRequestDelegate
    where TMessage : class
  {
    /// <summary>
    /// Creates an instance of <see cref="MessageRequestDelegate{TMessage}"/>.
    /// </summary>
    /// <param name="handlerImplementationType">The type of the handler that will handle the received message.</param>
    /// <param name="serializerOptions">The serializer options.</param>
    public MessageRequestDelegate(Type handlerImplementationType, JsonSerializerOptions serializerOptions)
      : base(handlerImplementationType, serializerOptions)
    {
    }

    protected override Type HandlerType => typeof(IMessageHandler<TMessage>);

    public override async Task ExecuteAsync(HttpContext context)
    {
      IMessageHandler<TMessage> handler;
      TMessage? message;
      try
      {
        handler = (IMessageHandler<TMessage>)context.RequestServices.GetRequiredService(HandlerImplementationType);
        message = await JsonSerializer.DeserializeAsync<TMessage>(context.Request.Body, SerializerOptions, context.RequestAborted);
        if (message is null)
        {
          throw await NullObjectDeserializedException.FromStreamAsync($"{nameof(JsonSerializer)} returned null on deserialization.", context.Request.Body);
        }
      }
      catch (Exception ex)
      {
        LogError(context.RequestServices, "Error in preparing a message handling action.", ex);
        // We return OK and log the error because there is something wrong with the app configuration, not with the business logic.
        // Dapr should not attempt to send the message again.
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        return;
      }

      try
      {
        await handler.HandleAsync(message, context.RequestAborted);
        context.Response.StatusCode = (int)HttpStatusCode.OK;
      }
      catch (Exception ex)
      {
        LogError(context.RequestServices, "Error in executing a message handling action.", ex);
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      }
    }

    private static void LogError(IServiceProvider serviceProvider, string message, Exception ex)
    {
      ILogger<MessageRequestDelegate<TMessage>>? logger = serviceProvider.GetService<ILogger<MessageRequestDelegate<TMessage>>>();
      logger?.LogError(ex, message);
    }
  }
}
