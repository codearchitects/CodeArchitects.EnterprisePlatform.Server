using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Metadata.Builder;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DaprActorsOptionsBuilderExtensions
{
  /// <summary>
  /// Registers an actor within the actor model. Scans the provided type's assembly for factories and implementations of that actor.
  /// </summary>
  /// <typeparam name="TActor">The type of the actor to register.</typeparam>
  /// <returns>The same builder.</returns>
  public static IDaprActorsOptionsBuilder AddActor<TActor>(this IDaprActorsOptionsBuilder builder)
    where TActor : class
  {
    return builder.AddActor(typeof(TActor));
  }

  /// <summary>
  /// Scans the provided assembly registering any classes decorated with the following attributes.
  /// <list type="bullet">
  /// <item><see cref="ActorAttribute"/></item>
  /// <item><see cref="ActorAttribute{TInterface}"/></item>
  /// <item><see cref="ActorImplementationAttribute"/></item>
  /// <item><see cref="ActorImplementationAttribute{TActor}"/></item>
  /// <item><see cref="ActorFactoryAttribute"/></item>
  /// <item><see cref="ActorFactoryAttribute{TActor}"/></item>
  /// </list>
  /// </summary>
  /// <typeparam name="TMarker">A type contained within assembly to scan.</typeparam>
  /// <returns>The same builder.</returns>
  public static IDaprActorsOptionsBuilder ScanAssembly<TMarker>(this IDaprActorsOptionsBuilder builder)
  {
    return builder.ScanAssembly(typeof(TMarker).Assembly);
  }

  /// <summary>
  /// Specifies an actor configuration type to use for registering actors.
  /// </summary>
  /// <typeparam name="TConfiguration">The configuration type. It must extend <see cref="ActorConfiguration"/> and have a parameterless constructor or a constructor with a single <see cref="IConfiguration"/> parameter.</typeparam>
  /// <returns>The same builder.</returns>
  public static IDaprActorsOptionsBuilder UseActorConfiguration<TConfiguration>(this IDaprActorsOptionsBuilder builder)
    where TConfiguration : ActorConfiguration
  {
    return builder.UseActorConfiguration(typeof(TConfiguration));
  }
}
