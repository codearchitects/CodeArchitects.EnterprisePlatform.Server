using CodeArchitects.Platform.Actors.Metadata.Implementation;
using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal abstract class ReflectionStateFieldMetadata : StateFieldMetadata
{
  protected ReflectionStateFieldMetadata(FieldInfo field)
    : base(field)
  {
  }

  protected abstract MemberInfo Member { get; }

  protected abstract Type MemberType { get; }

  public override Optional<object?> DefaultValue => Member.GetCustomAttribute<StateAttribute>().DefaultValue;

  public override bool IsActorIdSource(out PropertyInfo? actorIdProperty)
  {
    actorIdProperty = GetActorIdProperty();

    return Member.IsDefined(typeof(ActorIdAttribute)) || actorIdProperty is not null;
  }

  private PropertyInfo? GetActorIdProperty()
  {
    try
    {
      return MemberType
        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(property => property.IsDefined(typeof(ActorIdAttribute)))
        .SingleOrDefault();
    }
    catch (InvalidOperationException)
    {
      throw InvalidActorException.AmbiguousActorIdSource(Field.DeclaringType);
    }
  }
}
