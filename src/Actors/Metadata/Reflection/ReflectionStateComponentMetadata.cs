using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal class ReflectionStateComponentMetadata<TActor> : StateComponentMetadata<TActor>
  where TActor : class
{
  public ReflectionStateComponentMetadata(int index, MemberInfo member, Type type)
    : base(index, member, type)
  {
  }

  public override bool IsActorId => Member.IsDefined(typeof(ActorIdAttribute));

  public override Expression FactoryExpression
  {
    get
    {
      Optional<object?> optionalDefaultValue = GetOptionalDefaultValue();
      Debug.Assert(optionalDefaultValue.HasValue);

      return Expression.Constant(optionalDefaultValue.Value);
    }
  }

  public override bool TryGetDefaultValue(out object? defaultValue)
  {
    Optional<object?> optionalDefaultValue = GetOptionalDefaultValue();

    defaultValue = optionalDefaultValue.Value;
    return optionalDefaultValue.HasValue;
  }

  private Optional<object?> GetOptionalDefaultValue()
  {
    var stateAttribute = Member.GetCustomAttribute<StateAttribute>();
    if (stateAttribute == null)
    {
      return Optional<object?>.None;
    }

    return stateAttribute.DefaultValue;
  }
}
