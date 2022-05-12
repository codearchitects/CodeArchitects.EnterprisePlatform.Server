using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using CodeArchitects.Platform.Dapr.AspNetCore.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class DaprInfrastructureBuilderExtensions
{
  public static IDaprInfrastructureBuilder Configure(this IDaprInfrastructureBuilder builder, Action<IDaprConfigurationBuilder> configure)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    configure(builder.DaprConfigurationBuilder);

    return builder;
  }

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
