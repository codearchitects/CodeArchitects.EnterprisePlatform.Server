using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Implementation of <see cref="HandlerDelegate"/> for handlers that produce no results.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <typeparam name="THandler">The type of the handler.</typeparam>
internal class HandlerDelegate<TMessage, THandler> : HandlerDelegate
  where TMessage : class
  where THandler : IMessageHandler<TMessage>
{
  private readonly IEnumerable<OutputAction> _outputActions;

  /// <summary>
  /// Creates a new <see cref="HandlerDelegate{TMessage, THandler}"/>.
  /// </summary>
  /// <param name="outputActions">The pipeline's output actions.</param>
  public HandlerDelegate(IEnumerable<OutputAction> outputActions)
  {
    _outputActions = outputActions;
  }

  public override async Task HandleAsync(HttpContext context, JObject messageJson)
  {
    TMessage message;
    try
    {
      message = messageJson.ToObject<TMessage>()!;
    }
    catch
    {
      throw new InvalidMessageTypeException(typeof(TMessage));
    }

    THandler handler = context.RequestServices.GetRequiredService<THandler>();
    await handler.HandleAsync(message, context.RequestAborted);

    foreach (OutputAction action in _outputActions)
    {
      await action.ExecuteAsync(message, default(object), context.RequestAborted);
    }
  }
}
