using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class DefaultActorIdDescriptor : IActorIdDescriptor
{
  public static readonly DefaultActorIdDescriptor Instance = new();

  private DefaultActorIdDescriptor() { }

  public Type Type => typeof(string);

  public bool HasIdSource => false;

  public int StateIndex => -1;

  public PropertyInfo? IdProperty => null;
}
