using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Utils;

/// <summary>
/// Helper methods for logging.
/// </summary>
internal static class LogHelper
{
  /// <summary>
  /// Tries to create a logger from the service provider for the messaging category.
  /// </summary>
  /// <param name="services">The service provider.</param>
  /// <returns>The logger or <see cref="NullLogger.Instance"/>.</returns>
  public static ILogger CreateMessagingLogger(this IServiceProvider services)
  {
    return services.GetService<ILoggerFactory>()?.CreateLogger("CAEP-Messaging") ?? NullLogger.Instance;
  }
}
