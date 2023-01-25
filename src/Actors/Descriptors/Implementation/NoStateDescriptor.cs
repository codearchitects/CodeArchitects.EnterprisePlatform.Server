using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class NoStateDescriptor : IStateDescriptor
{
  public static readonly NoStateDescriptor Instance = new();

  private NoStateDescriptor() { }

  public Type StateType => typeof(NoState);

  public bool IsStateless => true;

  public bool IsVirtual => true;

  public IReadOnlyList<FieldInfo> Fields => Array.Empty<FieldInfo>();

  public IReadOnlyList<object?>? DefaultValues => Array.Empty<object?>();
}
