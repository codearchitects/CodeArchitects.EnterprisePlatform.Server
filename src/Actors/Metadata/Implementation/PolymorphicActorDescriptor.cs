using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class PolymorphicActorDescriptor<TActor, TState> : ActorDescriptor<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  private readonly List<IImplementationDescriptor<TActor, TState>> _implementations;
  private IImplementationDescriptor<TActor, TState>? _defaultImplementation;

  public PolymorphicActorDescriptor(
    Type interfaceType, bool isVirtual,
    Type activityBaseType,
    Action<TActor, TState> updateState,
    IStateDescriptor<TState> state,
    IActorIdDescriptor<TState> id,
    IActorFactoryDescriptor factory,
    IImplementationDescriptor<TActor, TState> baseImplementation,
    IReadOnlyCollection<IMethodDescriptor> methods,
    IReadOnlyCollection<IMethodDescriptor> activities,
    IReadOnlyCollection<IMessageHandlerDescriptor> messageHandlers)
    : base(interfaceType, isVirtual, activityBaseType, updateState, state, id, factory, baseImplementation, methods, activities, messageHandlers)
  {
    _implementations = new();
  }

  public override bool IsPolymorphic => true;

  public override IImplementationDescriptor<TActor, TState> DefaultImplementation => _defaultImplementation ?? _implementations[0];

  public override IReadOnlyCollection<IImplementationDescriptor<TActor, TState>> Implementations => _implementations;

  public override TActor CreateInstance(int implementationId, IServiceProvider services, TState state, IActorContext<TActor> context)
  {
    return _implementations[implementationId - 1].CreateInstance(services, state, context);
  }

  public override IImplementationDescriptor<TActor, TState> GetImplementation(Type implementationType)
  {
    foreach (IImplementationDescriptor<TActor, TState> implementation in _implementations)
    {
      if (implementation.Type == implementationType)
        return implementation;
    }

    throw new ArgumentException("Invalid implementation type.", nameof(implementationType));
  }

  public void AddImplementation(IImplementationDescriptor<TActor, TState> implementation, bool isDefault)
  {
    _implementations.Add(implementation);

    if (isDefault)
    {
      if (_defaultImplementation is not null)
        throw InvalidActorException.MultipleDefaultImplementations(ActorType);

      _defaultImplementation = implementation;
    }
  }
}
