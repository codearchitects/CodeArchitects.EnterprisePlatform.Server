using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

internal class StateComponentBuilder<TActor, TState> : StateComponentMetadata<TActor>, IStateComponentBuilder<TActor, TState>
  where TActor : class
{
  private Optional<TState> _defaultValue;
  private bool _isActorId;

  public StateComponentBuilder(int index, MemberInfo member, Type type)
    : base(index, member, type)
  {
  }

  public override bool IsActorId => _isActorId;

  public override bool HasDefaultValue(out object? defaultComponentValue)
  {
    if (_defaultValue.HasValue)
    {
      defaultComponentValue = _defaultValue.Value;
      return true;
    }

    defaultComponentValue = null;
    return false;
  }

  IStateComponentBuilder<TActor, TState> IStateComponentBuilder<TActor, TState>.HasDefaultValue(TState value)
  {
    _defaultValue = value;
    return this;
  }

  IStateComponentBuilder<TActor, TState> IStateComponentBuilder<TActor, TState>.IsActorId(bool isActorId = true)
  {
    _isActorId = isActorId;
    return this;
  }
}
