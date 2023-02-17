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

  public override bool IsActorIdSource(out PropertyInfo? actorIdProperty)
  {
    actorIdProperty = GetActorIdProperty();

    return Member.IsDefined(typeof(ActorIdAttribute)) || actorIdProperty is not null;
  }

  private PropertyInfo? GetActorIdProperty()
  {
    try
    {
      return Type
        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(property => property.IsDefined(typeof(ActorIdAttribute)))
        .SingleOrDefault();
    }
    catch (InvalidOperationException)
    {
      throw InvalidActorException.AmbiguousActorIdSource(Member.DeclaringType);
    }
  }
}
