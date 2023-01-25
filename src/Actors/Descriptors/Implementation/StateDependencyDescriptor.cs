using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class StateDependencyDescriptor : DependencyDescriptor, IStateDependencyDescriptor
{
  public StateDependencyDescriptor(ParameterInfo parameter, FieldInfo field, int categoryIndex)
    : base(parameter)
  {
    Field = field;
    CategoryIndex = categoryIndex;
  }

  public FieldInfo Field { get; }

  public override DependencyKind Kind => DependencyKind.State;

  public override int CategoryIndex { get; }

  public override void Accept(IDependencyDescriptorVisitor visitor)
  {
    visitor.VisitStateDependency(this);
  }
}
