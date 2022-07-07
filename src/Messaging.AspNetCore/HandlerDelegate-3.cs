using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Implementation of <see cref="HandlerDelegate"/> for handlers that produce results.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <typeparam name="THandler">The type of the handler.</typeparam>
internal class HandlerDelegate<TMessage, TResult, THandler> : HandlerDelegate
  where TMessage : class
  where TResult : class
  where THandler : IMessageHandler<TMessage, TResult>
{
  private readonly IEnumerable<OutputAction> _outputActions;

  /// <summary>
  /// Creates a new <see cref="HandlerDelegate{TMessage, TResult, THandler}"/> instance.
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
    TResult result = await handler.HandleAsync(message, context.RequestAborted);

    foreach (OutputAction action in _outputActions)
    {
      await action.ExecuteAsync(message, result, context.RequestAborted);
    }
  }
}
