using CodeArchitects.Platform.Actors.Metadata.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

internal class MethodMetadataBuilder : MethodMetadata, IMethodMetadataBuilder
{
  private bool _isStateless;

  public MethodMetadataBuilder(MethodInfo implementationMethod)
    : base(implementationMethod)
  {
  }

  public override bool IsStateless => _isStateless;

  public IMethodMetadataBuilder AsBuilder() => this;

  IMethodMetadataBuilder IMethodMetadataBuilder.IsStateless(bool isStateless = true)
  {
    _isStateless = isStateless;

    return this;
  }
}
