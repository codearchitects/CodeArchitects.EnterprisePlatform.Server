using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Dapr.AspNetCore;

/// <summary>
/// Extension methods for <see cref="IDaprInfrastructureBuilder"/>.
/// </summary>
public static class DaprInfrastructureBuilderExtensions
{
  /// <summary>
  /// Fluently sets the logger of the builder.
  /// </summary>
  /// <param name="builder">The builder.</param>
  /// <param name="logger">The logger.</param>
  /// <returns>The builder.</returns>
  public static IDaprInfrastructureBuilder UseLogger(this IDaprInfrastructureBuilder builder, ILogger logger)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));
    if (logger is null)
      throw new ArgumentNullException(nameof(logger));

    builder.Logger = logger;

    return builder;
  }
}
