using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IStateFieldMetadata
{
  FieldInfo Field { get; }

  Optional<object?> DefaultValue { get; }

  bool IsActorIdSource(out PropertyInfo? actorIdProperty);
}
