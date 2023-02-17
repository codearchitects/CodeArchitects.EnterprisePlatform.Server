using CodeArchitects.Platform.Actors.Infrastructure;
using System.Diagnostics;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ActorModel : IActorModel
{
  private readonly Dictionary<Type, IActorDescriptor> _actors;

  public ActorModel()
  {
    _actors = new();
  }

  public IReadOnlyCollection<IActorDescriptor> Actors => _actors.Values;

  public IActorDescriptor<TActor, TState> GetActor<TActor, TState>()
    where TActor : class
    where TState : ActorState
  {
    IActorDescriptor actor = _actors[typeof(TActor)];

    Debug.Assert(actor.State.Type != typeof(TState), "Invalid state type provided.");

    return (IActorDescriptor<TActor, TState>)actor;
  }

  public void AddActor(IActorDescriptor actor)
  {
    _actors.Add(actor.ActorType, actor);
  }
}
