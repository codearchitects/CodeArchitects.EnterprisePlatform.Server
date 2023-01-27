using CodeArchitects.Platform.Actors.Metadata.Implementation;
using CodeArchitects.Platform.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

internal class StateFieldMetadataBuilder<TState> : StateFieldMetadata, IStateFieldMetadataBuilder<TState>
{
  private Optional<object?> _defaultValue;
  private bool _isActorId;
  private PropertyInfo? _actorIdProperty;

  public StateFieldMetadataBuilder(FieldInfo field)
    : base(field)
  {
  }

  public override Optional<object?> DefaultValue => _defaultValue;

  public override bool IsActorIdSource(out PropertyInfo? actorIdProperty)
  {
    actorIdProperty = _actorIdProperty;

    return _isActorId || actorIdProperty is not null;
  }

  public IStateFieldMetadataBuilder<TState> AsBuilder() => this;

  IStateFieldMetadataBuilder<TState> IStateFieldMetadataBuilder<TState>.HasDefaultValue(TState defaultValue)
  {
    _defaultValue = defaultValue;

    return this;
  }

  IStateFieldMetadataBuilder<TState> IStateFieldMetadataBuilder<TState>.IsActorId()
  {
    _isActorId = true;
    _actorIdProperty = null;

    return this;
  }

  IStateFieldMetadataBuilder<TState> IStateFieldMetadataBuilder<TState>.IsActorIdSource<TProperty>(Expression<Func<TState, TProperty>> idPropertyExpression)
  {
    if (idPropertyExpression is null)
      throw new ArgumentNullException(nameof(idPropertyExpression));

    ParameterExpression stateParameter = idPropertyExpression.Parameters[0];
    if (idPropertyExpression.Body is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo actorIdProperty || memberExpression.Expression != stateParameter)
      throw new ArgumentException($"'{nameof(idPropertyExpression)}' must represent a property access on the state component type.", nameof(idPropertyExpression));

    _actorIdProperty = actorIdProperty;
    _isActorId = false;

    return this;
  }
}
