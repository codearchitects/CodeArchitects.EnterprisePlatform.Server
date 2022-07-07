using CodeArchitects.Platform.Messaging.Descriptors;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Descriptors;

internal static class ConfigurationHandlerDiagnostics
{
  /// <summary>
  /// Raised when a handler is not found by its fully qualified name.
  /// </summary>
  /// <param name="className">The name of the handler.</param>
  /// <returns>The diagnostics.</returns>
  public static HandlerDiagnostics HandlerNotFoundByName(string className)
    => new(100, null, "Could not find handler '{0}'", className);
}
