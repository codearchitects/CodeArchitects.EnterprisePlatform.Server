using CodeArchitects.Platform.Actors.Infrastructure;
using System.Diagnostics;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class OrdinaryActorDescriptor<TActor, TState> : ActorDescriptor<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  public OrdinaryActorDescriptor(
    Type interfaceType,
    bool isVirtual,
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
  }

  public override bool IsPolymorphic => false;

  public override IImplementationDescriptor<TActor, TState> DefaultImplementation => BaseImplementation;

  public override IReadOnlyCollection<IImplementationDescriptor<TActor, TState>> Implementations => new[] { BaseImplementation };

  public override TActor CreateInstance(int implementationId, IServiceProvider services, TState state, IActorContext<TActor> context)
  {
    Debug.Assert(implementationId == 0, "The actor was not supposed to be polymorphic.");

    return BaseImplementation.CreateInstance(services, state, context);
  }

  public override IImplementationDescriptor<TActor, TState> GetImplementation(Type implementationType)
  {
    Debug.Assert(implementationType == typeof(TActor), "The actor was not supposed to be polymorphic.");

    return BaseImplementation;
  }
}
