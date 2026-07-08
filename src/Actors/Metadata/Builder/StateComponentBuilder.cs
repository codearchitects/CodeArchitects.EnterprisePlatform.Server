using CodeArchitects.Platform.Actors.Metadata.Factory;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

internal class StateComponentBuilder<TActor, TStateComponent> : StateComponentMetadata<TActor>, IStateComponentBuilder<TActor, TStateComponent>
  where TActor : class
{
  private Func<TStateComponent>? _defaultValueFactory;
  private bool _isActorId;

  public StateComponentBuilder(int index, MemberInfo member, Type type)
    : base(index, member, type)
  {
  }

  public override bool IsActorId => _isActorId;

  public override Expression FactoryExpression
  {
    get
    {
      Debug.Assert(_defaultValueFactory != null);

      return Expression.Invoke(Expression.Constant(_defaultValueFactory));
    }
  }

  public IStateComponentBuilder<TActor, TStateComponent> AsBuilder() => this;

  public override bool TryGetDefaultValue(out object? defaultValue)
  {
    if (_defaultValueFactory is null)
    {
      defaultValue = null;
      return false;
    }

    defaultValue = _defaultValueFactory();
    return true;
  }

  IStateComponentBuilder<TActor, TStateComponent> IStateComponentBuilder<TActor, TStateComponent>.HasDefaultValue(TStateComponent value)
  {
    _defaultValueFactory = () => value;
    return this;
  }

  IStateComponentBuilder<TActor, TStateComponent> IStateComponentBuilder<TActor, TStateComponent>.HasDefaultValueFactory(Func<TStateComponent> valueFactory)
  {
    _defaultValueFactory = valueFactory;
    return this;
  }

  IStateComponentBuilder<TActor, TStateComponent> IStateComponentBuilder<TActor, TStateComponent>.IsActorId(bool isActorId)
  {
    _isActorId = isActorId;
    return this;
  }
}
