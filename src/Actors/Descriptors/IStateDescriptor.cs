using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IStateDescriptor
{
  Type Type { get; }

  IReadOnlyList<FieldInfo> Fields { get; }

  object? DefaultValue { get; }
}
