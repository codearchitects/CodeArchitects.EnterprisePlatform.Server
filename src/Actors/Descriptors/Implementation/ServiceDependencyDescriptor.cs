using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ServiceDependencyDescriptor : DependencyDescriptor, IServiceDependencyDescriptor
{
  public ServiceDependencyDescriptor(ParameterInfo parameter, int categoryIndex)
    : base(parameter)
  {
    CategoryIndex = categoryIndex;
  }

  public override DependencyKind Kind => DependencyKind.Service;

  public override int CategoryIndex { get; }

  public bool IsOptional { get; set; }

  public override void Accept(IDependencyDescriptorVisitor visitor)
  {
    visitor.VisitServiceDependency(this);
  }
}
