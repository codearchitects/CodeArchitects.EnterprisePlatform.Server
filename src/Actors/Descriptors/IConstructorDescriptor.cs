using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IConstructorDescriptor
{
  ConstructorInfo Constructor { get; }

  IReadOnlyList<IDependencyDescriptor> Dependencies { get; }

  IContextDependencyDescriptor? ContextDependency { get; }

  IReadOnlyList<IServiceDependencyDescriptor> ServiceDependencies { get; }

  IReadOnlyList<IStateDependencyDescriptor> StateDependencies { get; }
}
