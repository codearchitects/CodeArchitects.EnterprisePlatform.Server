using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IStateDependencyDescriptor : IDependencyDescriptor
{
  FieldInfo Field { get; }

  Optional<object?> Default { get; }
}
