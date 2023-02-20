using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Reflection;

internal class ReflectionStateComponentMetadata<TActor> : StateComponentMetadata<TActor>
  where TActor : class
{
  public ReflectionStateComponentMetadata(int index, MemberInfo member, Type type)
    : base(index, member, type)
  {
  }

  public override bool IsActorId => Member.IsDefined(typeof(ActorIdAttribute));

  public override bool HasDefaultValue(out object? defaultComponentValue)
  {
    Optional<object?> defaultValue = Member.GetCustomAttribute<StateAttribute>().DefaultValue;

    if (defaultValue.HasValue)
    {
      defaultComponentValue = defaultValue.Value;
      return true;
    }

    defaultComponentValue = null;
    return false;
  }
}
