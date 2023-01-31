using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IStateDescriptor
{
  Type StateType { get; }

  FieldInfo? DiscriminatorField { get; }

  bool IsStateless { get; }

  [MemberNotNullWhen(true, nameof(DefaultValues))]
  bool IsVirtual { get; }

  IReadOnlyList<FieldInfo> Fields { get; }

  IReadOnlyList<object?>? DefaultValues { get; }
}
