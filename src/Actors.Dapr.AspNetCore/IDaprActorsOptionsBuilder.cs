using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Metadata.Builder;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A builder that can be used to configure the options of an actor model.
/// </summary>
public interface IDaprActorsOptionsBuilder
{
  /// <summary>
  /// Configures the Dapr actor runtime.
  /// </summary>
  /// <param name="configure">An action used to configure actor options.</param>
  /// <returns>The same builder.</returns>
  IDaprActorsOptionsBuilder ConfigureRuntimeOptions(Action<Dapr.Actors.Runtime.ActorRuntimeOptions> configure);

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
  /// <param name="assembly">The assembly to scan.</param>
  /// <returns>The same builder.</returns>
  IDaprActorsOptionsBuilder ScanAssembly(Assembly assembly);

  /// <summary>
  /// Registers an actor within the actor model. Scans the provided type's assembly for factories and implementations of that actor.
  /// </summary>
  /// <param name="actorType">The type of the actor to register.</param>
  /// <returns>The same builder.</returns>
  IDaprActorsOptionsBuilder AddActor(Type actorType);

  /// <summary>
  /// Specifies an actor configuration type to use for registering actors.
  /// </summary>
  /// <param name="actorConfigurationType">The configuration type. It must extend <see cref="ActorConfiguration"/> and have a parameterless constructor or a constructor with a single <see cref="IConfiguration"/> parameter.</param>
  /// <returns>The same builder.</returns>
  IDaprActorsOptionsBuilder UseActorConfiguration(Type actorConfigurationType);

  /// <summary>
  /// Specifies the specific <see cref="Dapr.Actors.Runtime.ActorRuntimeOptions"/> to use for an actor type.
  /// </summary>
  /// <param name="actorType">The actor type to configure with the specified options.</param>
  /// <param name="options">The options to use for the actor.</param>
  /// <returns>The same builder.</returns>
  IDaprActorsOptionsBuilder UseRuntimeOptions(Type actorType, Dapr.Actors.Runtime.ActorRuntimeOptions options);

  /// <summary>
  /// Specifies whether any dynamically-generated code can access private methods of the actor classes.
  /// This is required, for example, when scheduling executions of private methods.
  /// </summary>
  /// <param name="accessPrivates"><see langword="true"/> if private methods can be accessed, otherwise <see langword="false"/>.</param>
  /// <returns>The same builder.</returns>
  IDaprActorsOptionsBuilder AccessPrivates(bool accessPrivates = true);
}
