using CodeArchitects.Platform.Actors.Infrastructure;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class StateDescriptor<TState> : IStateDescriptor<TState>
  where TState : ActorState
{
  private readonly Func<TState> _defaultStateFactory;

  public StateDescriptor(Func<TState> defaultStateFactory, IReadOnlyList<FieldInfo> fields)
  {
    _defaultStateFactory = defaultStateFactory;
    Fields = fields;
  }

  public Type Type => typeof(TState);

  public IReadOnlyList<FieldInfo> Fields { get; }

  public TState GetDefaultValue() => _defaultStateFactory();

  object IStateDescriptor.GetDefaultValue() => GetDefaultValue();
}
