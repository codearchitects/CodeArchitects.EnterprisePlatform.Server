using CodeArchitects.Platform.Actors.Descriptors.Builder;

namespace CodeArchitects.Platform.Actors.Dapr.AspNetCore;

public static class DaprActorsOptionsBuilderExtensions
{
  public static IDaprActorsOptionsBuilder AddActor<TActor>(this IDaprActorsOptionsBuilder builder)
    where TActor : class
  {
    return builder.AddActor(typeof(TActor));
  }

  public static IDaprActorsOptionsBuilder ScanAssembly<TMarker>(this IDaprActorsOptionsBuilder builder)
  {
    return builder.ScanAssembly(typeof(TMarker).Assembly);
  }

  public static IDaprActorsOptionsBuilder UseActorConfiguration<TConfiguration>(this IDaprActorsOptionsBuilder builder)
    where TConfiguration : ActorConfiguration
  {
    return builder.UseActorConfiguration(typeof(TConfiguration));
  }
}
