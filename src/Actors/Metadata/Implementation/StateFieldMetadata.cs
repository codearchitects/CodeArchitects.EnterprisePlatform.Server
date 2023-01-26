using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal abstract class StateFieldMetadata : IStateFieldMetadata
{
  protected StateFieldMetadata(FieldInfo field)
  {
    Field = field;
  }

  public FieldInfo Field { get; }

  public abstract Optional<object?> DefaultValue { get; }

  public abstract bool IsActorIdSource(out PropertyInfo? actorIdProperty);
}
