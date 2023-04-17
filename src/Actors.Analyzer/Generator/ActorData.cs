using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Generator;

internal class ActorData : TypeData 
{
  private readonly IReadOnlyDictionary<ITypeSymbol, Location>? _baseTypeLocations;

  public ActorData(INamedTypeSymbol actorType, AttributeData? actorAttribute, AttributeData? genericActorAttribute, AttributeData? virtualAttribute, AttributeData? idTypeAttribute, AttributeData? genericIdTypeAttribute, IReadOnlyDictionary<ITypeSymbol, Location>? baseTypeLocations)
  {
    ActorType = actorType;
    ActorAttribute = actorAttribute;
    GenericActorAttribute = genericActorAttribute;
    VirtualAttribute = virtualAttribute;
    IdTypeAttribute = idTypeAttribute;
    GenericIdTypeAttribute = genericIdTypeAttribute;
    _baseTypeLocations = baseTypeLocations;
  }

  public INamedTypeSymbol ActorType { get; }
  public AttributeData? ActorAttribute { get; }
  public AttributeData? GenericActorAttribute { get; }
  public AttributeData? VirtualAttribute { get; }
  public AttributeData? IdTypeAttribute { get; }
  public AttributeData? GenericIdTypeAttribute { get; }

  public Location GetBaseTypeLocation(ITypeSymbol baseType)
  {
    if (_baseTypeLocations is not null && _baseTypeLocations.TryGetValue(baseType, out Location? location))
      return location;

    return Type.Locations[0];
  }

  protected override INamedTypeSymbol Type => ActorType;

  public override void AddTo(in ActorDescriptorFactory factory) => factory.AddActor(this);
}
