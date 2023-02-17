using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class PropertyActorIdDescriptor : IActorIdDescriptor
{
  public PropertyActorIdDescriptor(PropertyInfo idProperty, int stateIndex)
  {
    IdProperty = idProperty;
    StateIndex = stateIndex;
  }

  public Type Type => IdProperty.PropertyType;

  public bool HasIdSource => true;

  public int StateIndex { get; }

  public PropertyInfo IdProperty { get; }
}
