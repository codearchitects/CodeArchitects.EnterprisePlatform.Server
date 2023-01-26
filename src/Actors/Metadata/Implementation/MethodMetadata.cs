using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal abstract class MethodMetadata : IMethodMetadata
{
  protected MethodMetadata(MethodInfo implementationMethod)
  {
    ImplementationMethod = implementationMethod;
  }

  public MethodInfo ImplementationMethod { get; }

  public abstract bool IsStateless { get; }
}
