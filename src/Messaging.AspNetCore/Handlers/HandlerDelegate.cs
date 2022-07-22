using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Handlers;

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
}
