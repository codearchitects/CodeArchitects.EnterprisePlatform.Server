using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Dapr.AspNetCore;

public static class DaprInfrastructureBuilderExtensions
{
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
