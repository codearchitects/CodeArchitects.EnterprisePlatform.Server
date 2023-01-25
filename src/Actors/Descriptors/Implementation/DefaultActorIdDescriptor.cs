using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class DefaultActorIdDescriptor : ActorIdDescriptor
{
  public static readonly DefaultActorIdDescriptor Instance = new();

  private DefaultActorIdDescriptor() { }

  public override Type IdType => typeof(string);

  public override bool HasIdSource => false;

  public override IStateDependencyDescriptor? StateDependency => null;

  public override PropertyInfo? StateProperty => null;
}
