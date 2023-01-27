using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ServiceDependencyDescriptor : DependencyDescriptor, IServiceDependencyDescriptor
{
  public ServiceDependencyDescriptor(ParameterInfo parameter)
    : base(parameter)
  {
  }

  public override DependencyKind Kind => DependencyKind.Service;

  public bool IsOptional { get; set; }

  public override void Accept(IDependencyDescriptorVisitor visitor)
  {
    visitor.VisitServiceDependency(this);
  }
}
