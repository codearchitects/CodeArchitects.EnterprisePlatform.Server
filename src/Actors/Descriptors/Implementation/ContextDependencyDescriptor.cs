using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ContextDependencyDescriptor : DependencyDescriptor, IContextDependencyDescriptor
{
  public ContextDependencyDescriptor(ParameterInfo parameter)
    : base(parameter)
  {
  }

  public override DependencyKind Kind => DependencyKind.Context;

  public override int CategoryIndex => 0;

  public override void Accept(IDependencyDescriptorVisitor visitor)
  {
    visitor.VisitContextDependency(this);
  }
}
