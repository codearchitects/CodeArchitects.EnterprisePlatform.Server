using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class AbstractBaseImplementationDescriptor<TActor, TState> : IImplementationDescriptor<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  public int Id => 0;

  public Type Type => typeof(TActor);

  public TActor CreateInstance(IServiceProvider services, TState state, IActorContext<TActor> context)
  {
    throw new InvalidOperationException($"Cannot create an instance of abstract type '{Type.Name}'.");
  }
}
