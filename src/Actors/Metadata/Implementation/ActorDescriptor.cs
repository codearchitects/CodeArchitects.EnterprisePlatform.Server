using CodeArchitects.Platform.Actors.Infrastructure;
using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal abstract class ActorDescriptor<TActor, TState> : IActorDescriptor<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  private readonly Action<TActor, TState> _updateState;

  protected ActorDescriptor(
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
  {
    InterfaceType = interfaceType;
    IsVirtual = isVirtual;
    ActivityBaseType = activityBaseType;
    _updateState = updateState;
    State = state;
    Id = id;
    Factory = factory;
    BaseImplementation = baseImplementation;
    Methods = methods;
    Activities = activities;
    MessageHandlers = messageHandlers;

    JsonSerializerOptions = new()
    {
      IgnoreReadOnlyProperties = true,
      TypeInfoResolver = new ActorJsonTypeInfoResolver(this)
    };
  }

  public abstract bool IsPolymorphic { get; }

  public Type InterfaceType { get; }

  public Type ActorType => typeof(TActor);

  public Type ActivityBaseType { get; }

  public bool IsVirtual { get; }

  public JsonSerializerOptions JsonSerializerOptions { get; }

  public IReadOnlyCollection<IMethodDescriptor> Methods { get; }

  public IReadOnlyCollection<IMethodDescriptor> Activities { get; }

  public IActorIdDescriptor<TState> Id { get; }

  public IActorFactoryDescriptor Factory { get; }

  public IImplementationDescriptor<TActor, TState> BaseImplementation { get; }

  public abstract IImplementationDescriptor<TActor, TState> DefaultImplementation { get; }

  public abstract IReadOnlyCollection<IImplementationDescriptor<TActor, TState>> Implementations { get; }

  public IStateDescriptor<TState> State { get; }

  public IReadOnlyCollection<IMessageHandlerDescriptor> MessageHandlers { get; }

  IImplementationDescriptor IActorDescriptor.BaseImplementation => BaseImplementation;

  IImplementationDescriptor IActorDescriptor.DefaultImplementation => DefaultImplementation;

  IReadOnlyCollection<IImplementationDescriptor> IActorDescriptor.Implementations => Implementations;

  IStateDescriptor IActorDescriptor.State => State;

  IActorIdDescriptor IActorDescriptor.Id => Id;

  public abstract TActor CreateInstance(int implementationId, IServiceProvider services, TState state, IActorContext<TActor> context);

  public abstract IImplementationDescriptor<TActor, TState> GetImplementation(Type implementationType);

  public void UpdateState(TActor actor, TState state)
  {
    _updateState(actor, state);
  }

  IImplementationDescriptor IActorDescriptor.GetImplementation(Type implementationType)
  {
    return GetImplementation(implementationType);
  }
}
