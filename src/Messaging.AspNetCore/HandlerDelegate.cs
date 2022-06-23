using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Delegates the execution of a message to its handler and executes output bindings.
/// </summary>
internal abstract class HandlerDelegate
{
  /// <summary>
  /// Executes the handling pipeline.
  /// </summary>
  /// <param name="context">The context of the request.</param>
  /// <param name="messageJson">The json message.</param>
  /// <returns></returns>
  public abstract Task HandleAsync(HttpContext context, JObject messageJson);

  /// <summary>
  /// Creates a new <see cref="HandlerDelegate"/> for a message handler that produces no result.
  /// </summary>
  /// <param name="outputActions">The pipeline's output actions.</param>
  /// <param name="messageType">The type of the message.</param>
  /// <param name="handlerType">The type of the handler.</param>
  /// <returns>The created instance.</returns>
  public static HandlerDelegate CreateNoResult(IEnumerable<OutputAction> outputActions, Type messageType, Type handlerType)
  {
    Type delegateType = typeof(HandlerDelegate<,>).MakeGenericType(messageType, handlerType);
    return (HandlerDelegate)Activator.CreateInstance(delegateType, new[] { outputActions })!;
  }

  /// <summary>
  /// Creates a new <see cref="HandlerDelegate"/> for a message handler that produces a result.
  /// </summary>
  /// <param name="outputActions">The pipeline's output actions.</param>
  /// <param name="messageType">The type of the message.</param>
  /// <param name="resultType">The type of the result.</param>
  /// <param name="handlerType">The type of the handler.</param>
  /// <returns></returns>
  public static HandlerDelegate CreateWithResult(IEnumerable<OutputAction> outputActions, Type messageType, Type resultType, Type handlerType)
  {
    Type delegateType = typeof(HandlerDelegate<,,>).MakeGenericType(messageType, resultType, handlerType);
    return (HandlerDelegate)Activator.CreateInstance(delegateType, new[] { outputActions })!;
  }
}
