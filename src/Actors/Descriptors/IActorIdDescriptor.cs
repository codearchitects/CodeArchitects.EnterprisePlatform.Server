using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorIdDescriptor
{
  Type Type { get; }

  [MemberNotNullWhen(true, nameof(StateDependency))]
  bool HasIdSource { get; }
  
  IStateDependencyDescriptor? StateDependency { get; }

  PropertyInfo? IdProperty { get; }
}
