using CodeArchitects.Platform.Actors.Metadata.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal class ReflectionMethodMetadata : MethodMetadata
{
  public ReflectionMethodMetadata(MethodInfo implementationMethod)
    : base(implementationMethod)
  {
  }

  public override bool IsStateless => ImplementationMethod.IsDefined(typeof(StatelessAttribute));
}
