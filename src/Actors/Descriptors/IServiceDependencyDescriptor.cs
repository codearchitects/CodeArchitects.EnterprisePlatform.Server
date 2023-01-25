namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IServiceDependencyDescriptor : IDependencyDescriptor
{
  bool IsOptional { get; }
}
