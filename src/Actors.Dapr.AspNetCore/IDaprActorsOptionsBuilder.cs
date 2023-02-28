using Dapr.Actors.Runtime;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public interface IDaprActorsOptionsBuilder
{
  IDaprActorsOptionsBuilder ConfigureRuntimeOptions(Action<ActorRuntimeOptions> configure);

  IDaprActorsOptionsBuilder ScanAssembly(Assembly assembly);

  IDaprActorsOptionsBuilder AddActor(Type actorType);

  IDaprActorsOptionsBuilder UseActorConfiguration(Type actorConfigurationType);

  IDaprActorsOptionsBuilder UseRuntimeOptions(Type actorType, ActorRuntimeOptions options);

  IDaprActorsOptionsBuilder AccessPrivates(bool accessPrivates = true);
}
