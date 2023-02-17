using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

internal class StateComponentBuilder<TActor, TState> : StateComponentMetadata<TActor>, IStateComponentBuilder<TActor, TState>
  where TActor : class
{
  private Optional<TState> _defaultValue;
  private bool _isActorId;
  private PropertyInfo? _idProperty;

  public StateComponentBuilder(int index, MemberInfo member, Type type)
    : base(index, member, type)
  {
  }

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

  public override bool IsActorIdSource(out PropertyInfo? actorIdProperty)
  {
    if (_isActorId)
    {
      actorIdProperty = null;
      return true;
    }

    actorIdProperty = _idProperty;
    return actorIdProperty is not null;
  }

  public IStateComponentBuilder<TActor, TState> HasDefaultValue(TState value)
  {
    _defaultValue = value;
    return this;
  }

  public IStateComponentBuilder<TActor, TState> IsActorId(bool isActorId = true)
  {
    _isActorId = isActorId;
    _idProperty = null;
    return this;
  }

  public IStateComponentBuilder<TActor, TState> IsActorIdSource<TProperty>(Expression<Func<TState, TProperty>> memberExpression)
  {
    if (memberExpression is null)
      throw new ArgumentNullException(nameof(memberExpression));

    if (memberExpression.Body is not MemberExpression body || body.Expression != memberExpression.Parameters[0] || body.Member is not PropertyInfo idProperty)
      throw new ArgumentException("The expression must represent a property access.", nameof(memberExpression));

    _isActorId = false;
    _idProperty = idProperty;
    return this;
  }

  public IStateComponentBuilder<TActor, TState> IsActorIdSource(string memberName)
  {
    if (memberName is null)
      throw new ArgumentNullException(nameof(memberName));

    _isActorId = false;
    _idProperty = typeof(TState).GetRequiredProperty(
      name: memberName,
      bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    return this;
  }
}
