using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ComponentActorIdDescriptor : IActorIdDescriptor
{
  public ComponentActorIdDescriptor(Type type, int stateIndex)
  {
    Type = type;
    StateIndex = stateIndex;
  }

  public Type Type { get; }

  public bool HasIdSource => true;

  public int StateIndex { get; }

  public PropertyInfo? IdProperty => null;
}
