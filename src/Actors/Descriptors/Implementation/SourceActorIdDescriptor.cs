using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class SourceActorIdDescriptor : ActorIdDescriptor
{
  public SourceActorIdDescriptor(IStateDependencyDescriptor stateDependency, PropertyInfo? stateProperty)
  {
    StateDependency = stateDependency;
    StateProperty = stateProperty;
  }

  public override Type IdType => StateProperty is { } property
    ? property.PropertyType
    : StateDependency.Type;

  public override bool HasIdSource => true;

  public override IStateDependencyDescriptor StateDependency { get; }

  public override PropertyInfo? StateProperty { get; }
}
