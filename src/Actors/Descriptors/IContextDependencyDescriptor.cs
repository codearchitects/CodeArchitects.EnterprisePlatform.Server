namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IContextDependencyDescriptor : IDependencyDescriptor
{
  Type ImplementationType { get; }
}
