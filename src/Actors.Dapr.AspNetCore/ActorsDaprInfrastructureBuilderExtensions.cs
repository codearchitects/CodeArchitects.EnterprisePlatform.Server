using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Dapr.AspNetCore;
using CodeArchitects.Platform.Dapr.AspNetCore;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IDaprInfrastructureBuilder"/> for configuring actors.
/// </summary>
public static class ActorsDaprInfrastructureBuilderExtensions
{
  /// <summary>
  /// Adds the capabilities of running actors within an applications using the Dapr.Actors library.
  /// </summary>
  /// <remarks>
  /// This method will scan the calling assembly and register any classes decorated with <see cref="ActorAttribute"/> or <see cref="ActorAttribute{TInterface}"/>.
  /// </remarks>
  /// <param name="builder">The Dapr infrastructure builder.</param>
  /// <returns>The same builder.</returns>
  public static IDaprInfrastructureBuilder AddActors(this IDaprInfrastructureBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return AddActorsCore(builder, options => options.ScanAssembly(Assembly.GetCallingAssembly()));
  }

  /// <summary>
  /// Adds the capabilities of running actors within an applications using the Dapr.Actors library.
  /// </summary>
  /// <param name="builder">The Dapr infrastructure builder.</param>
  /// <param name="configure">An action used to configure the actor model.</param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public static IDaprInfrastructureBuilder AddActors(this IDaprInfrastructureBuilder builder, Action<IDaprActorsOptionsBuilder> configure)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    return AddActorsCore(builder, configure);
  }

  private static IDaprInfrastructureBuilder AddActorsCore(this IDaprInfrastructureBuilder builder, Action<IDaprActorsOptionsBuilder> configure)
  {
    DaprActorsOptionsBuilder options = new();
    configure(options);

    options.RegisterServices(builder.Services, builder.Configuration);
    return builder;
  }
}
