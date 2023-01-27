using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class StateDependencyDescriptor : DependencyDescriptor, IStateDependencyDescriptor
{
  public StateDependencyDescriptor(ParameterInfo parameter, FieldInfo field, int fieldIndex)
    : base(parameter)
  {
    Field = field;
    FieldIndex = fieldIndex;
  }

  public override DependencyKind Kind => DependencyKind.State;

  public FieldInfo Field { get; }

  public int FieldIndex { get; }

  public override void Accept(IDependencyDescriptorVisitor visitor)
  {
    visitor.VisitStateDependency(this);
  }
}
