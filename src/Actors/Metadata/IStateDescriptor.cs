using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IStateDescriptor
{
  Type Type { get; }

  IReadOnlyList<FieldInfo> Fields { get; }

  object GetDefaultValue();
}
