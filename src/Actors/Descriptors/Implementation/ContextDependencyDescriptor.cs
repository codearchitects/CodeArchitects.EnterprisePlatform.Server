using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal abstract class ContextDependencyDescriptor : DependencyDescriptor, IContextDependencyDescriptor
{
  public ContextDependencyDescriptor(ParameterInfo parameter)
    : base(parameter)
  {
  }

  public override DependencyKind Kind => DependencyKind.Context;

  public abstract Type ImplementationType { get; }

  public override void Accept(IDependencyDescriptorVisitor visitor)
  {
    visitor.VisitContextDependency(this);
  }
}
