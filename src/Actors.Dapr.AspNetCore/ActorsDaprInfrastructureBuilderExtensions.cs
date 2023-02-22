using CodeArchitects.Platform.Dapr.AspNetCore;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.AspNetCore;

public static class ActorsDaprInfrastructureBuilderExtensions
{
  public static IDaprInfrastructureBuilder AddActors(this IDaprInfrastructureBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return AddActorsCore(builder, options => options.ScanAssembly(Assembly.GetCallingAssembly()));
  }

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
