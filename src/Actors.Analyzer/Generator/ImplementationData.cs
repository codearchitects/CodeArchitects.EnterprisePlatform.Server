using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Generator;

internal sealed class ImplementationData : TypeData
{
  public ImplementationData(INamedTypeSymbol implementationType, AttributeData? implementationAttribute, AttributeData? genericImplementationAttribute)
  {
    ImplementationType = implementationType;
    ImplementationAttribute = implementationAttribute;
    GenericImplementationAttribute = genericImplementationAttribute;
  }

  public INamedTypeSymbol ImplementationType { get; }
  public AttributeData? ImplementationAttribute { get; }
  public AttributeData? GenericImplementationAttribute { get; }

  protected override INamedTypeSymbol Type => ImplementationType;

  public override void AddTo(in ActorDescriptorFactory factory) => factory.AddImplementation(this);
}
