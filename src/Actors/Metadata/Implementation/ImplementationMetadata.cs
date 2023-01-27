using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal abstract class ImplementationMetadata : IImplementationMetadata
{
  private readonly Dictionary<MethodInfo, IMethodMetadata> _methods;

  public ImplementationMetadata()
  {
    _methods = new();
  }

  public abstract bool IsDefault { get; }

  public abstract Type ImplementationType { get; }

  public abstract ConstructorInfo? Constructor { get; }

  public abstract bool HasStateFields { get; }

  public IMethodMetadata GetMethodMetadata(MethodInfo implementationMethod)
  {
    if (_methods.TryGetValue(implementationMethod, out IMethodMetadata metadata))
      return metadata;

    return DefaultMethodMetadata.Instance;
  }

  protected void AddMethodMetadata(MethodMetadata methodMetadata)
  {
    _methods.Add(methodMetadata.ImplementationMethod, methodMetadata);
  }
}
