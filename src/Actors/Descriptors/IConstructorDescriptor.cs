using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IConstructorDescriptor
{
  ConstructorInfo Constructor { get; }

  IReadOnlyList<IDependencyDescriptor> Dependencies { get; }

  IReadOnlyList<IContextDependencyDescriptor> ContextDependencies { get; }

  IReadOnlyList<IServiceDependencyDescriptor> ServiceDependencies { get; }

  IReadOnlyList<IStateDependencyDescriptor> StateDependencies { get; }
}
