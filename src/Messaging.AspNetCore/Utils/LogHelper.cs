using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Utils;

/// <summary>
/// Helper methods for logging.
/// </summary>
internal static class LogHelper
{
  /// <summary>
  /// Tries to retrieve a logger from the service provider.
  /// </summary>
  /// <param name="services">The service provider.</param>
  /// <returns>The logger or null.</returns>
  public static ILogger? TryGetLogger(this IServiceProvider services)
  {
    return services.GetService<ILoggerFactory>()?.CreateLogger("CAEP-Messaging");
  }
}
