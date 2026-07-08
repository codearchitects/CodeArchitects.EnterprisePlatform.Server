using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Handlers;

/// <summary>
/// Base implementation of <see cref="HandlerDelegate"/>.
/// </summary>
/// <typeparam name="THandler">The type of the handler.</typeparam>
/// <typeparam name="TMessage">The type of the message.</typeparam>
internal abstract class BaseHandlerDelegate<THandler, TMessage> : HandlerDelegate
    where THandler : notnull
{
  public sealed override Task HandleAsync(HttpContext context, JObject messageJson)
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
    return HandleAsync(handler, message, context.RequestAborted);
  }

  protected async Task ExecuteOutputActionsAsync<TResult>(IEnumerable<OutputAction> outputActions, TMessage message, TResult? result, CancellationToken cancellationToken)
  {
    foreach (OutputAction action in outputActions)
    {
      await action.ExecuteAsync(message, result, cancellationToken);
    }
  }

  protected abstract Task HandleAsync(THandler handler, TMessage message, CancellationToken cancellationToken);
}
