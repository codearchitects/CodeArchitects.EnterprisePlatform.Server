using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class ImplementationDescriptor<TActor, TState> : IImplementationDescriptor<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  private readonly ImplementationFactory<TActor, TState> _implementationFactory;

  public ImplementationDescriptor(int id, ImplementationFactory<TActor, TState> implementationFactory, Type type)
  {
    Id = id;
    _implementationFactory = implementationFactory;
    Type = type;
  }

  public int Id { get; }

  public Type Type { get; }

  public TActor CreateInstance(IServiceProvider services, TState state, IActorContext<TActor> context)
  {
    return _implementationFactory(services, state, context);
  }
}
