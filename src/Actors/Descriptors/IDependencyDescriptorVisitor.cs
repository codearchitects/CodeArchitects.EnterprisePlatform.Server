namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IDependencyDescriptorVisitor
{
  void VisitStateDependency(IStateDependencyDescriptor dependency);

  void VisitServiceDependency(IServiceDependencyDescriptor dependency);
  
  void VisitContextDependency(IContextDependencyDescriptor dependency);
}
