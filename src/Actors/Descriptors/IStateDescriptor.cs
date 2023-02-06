using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IStateDescriptor
{
  Type Type { get; }

  FieldInfo? DiscriminatorField { get; }

  IReadOnlyList<FieldInfo> StateFields { get; }

  object? DefaultValue { get; }
}
