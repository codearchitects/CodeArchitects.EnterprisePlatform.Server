using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Generator;

internal sealed class FactoryData : TypeData
{
  public FactoryData(INamedTypeSymbol factoryType, AttributeData? factoryAttribute, AttributeData? genericFactoryAttribute)
  {
    FactoryType = factoryType;
    FactoryAttribute = factoryAttribute;
    GenericFactoryAttribute = genericFactoryAttribute;
  }

  public INamedTypeSymbol FactoryType { get; }
  public AttributeData? FactoryAttribute { get; }
  public AttributeData? GenericFactoryAttribute { get; }

  protected override INamedTypeSymbol Type => FactoryType;

  public override void AddTo(in ActorDescriptorFactory factory) => factory.AddFactory(this);
}
