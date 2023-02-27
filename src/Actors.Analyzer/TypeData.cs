using CodeArchitects.Platform.Actors.Analyzer.Descriptors;
using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer;

internal record TypeData(INamedTypeSymbol Type);

internal record ActorData(
  INamedTypeSymbol Type,
  AttributeData? ActorAttribute,
  AttributeData? GenericActorAttribute,
  AttributeData? VirtualAttribute) : TypeData(Type)
{
  public INamedTypeSymbol? GetSpecifiedInterfaceType(in ActorDescriptorFactory factory)
  {
    ITypeSymbol? specifiedInterface;
    AttributeData attribute;

    if (GenericActorAttribute is { } genericActorAttribute)
    {
      if (ActorAttribute is { } actorAttribute)
      {
        factory.DuplicateActorAttribute(actorAttribute);
      }

      attribute = genericActorAttribute;
      specifiedInterface = GetFromGenericAttribute(genericActorAttribute);
    }
    else
    {
      attribute = ActorAttribute!;
      specifiedInterface = GetFromNonGenericAttribute(attribute);
    }

    if (specifiedInterface is null)
      return null;

    if (specifiedInterface.TypeKind is not TypeKind.Interface)
    {
      factory.InterfaceTypeIsNotAnInterface(attribute, specifiedInterface);
      return null;
    }

    return (INamedTypeSymbol)specifiedInterface;

    static ITypeSymbol? GetFromNonGenericAttribute(AttributeData actorAttribute)
    {
      foreach (KeyValuePair<string, TypedConstant> namedArgument in actorAttribute.NamedArguments)
      {
        if (namedArgument.Key is "InterfaceType")
          return namedArgument.Value.Value as ITypeSymbol;
      }

      return null;
    }

    static ITypeSymbol GetFromGenericAttribute(AttributeData actorAttribute)
    {
      return actorAttribute.AttributeClass!.TypeArguments[0];
    }
  }
}

internal record FactoryData(
  INamedTypeSymbol Type,
  AttributeData? FactoryAttribute,
  AttributeData? GenericFactoryAttribute) : TypeData(Type)
{
  public ITypeSymbol GetSpecifiedActorType(in ActorDescriptorFactory factory)
  {
    ITypeSymbol? specifiedActor;

    if (GenericFactoryAttribute is { } genericFactoryAttribute)
    {
      if (FactoryAttribute is { } factoryAttribute)
      {
        factory.DuplicateActorFactoryAttribute(factoryAttribute);
      }

      specifiedActor = genericFactoryAttribute.AttributeClass!.TypeArguments[0];
    }
    else
    {
      specifiedActor = (ITypeSymbol)FactoryAttribute!.ConstructorArguments[0].Value!;
    }

    return specifiedActor;
  }
}

internal record ImplementationData(
  INamedTypeSymbol Type,
  AttributeData? ImplementationAttribute,
  AttributeData? GenericImplementationAttribute) : TypeData(Type)
{
  public ITypeSymbol GetSpecifiedActorType(in ActorDescriptorFactory factory)
  {
    ITypeSymbol? specifiedActor;

    if (GenericImplementationAttribute is { } genericImplementationAttribute)
    {
      if (ImplementationAttribute is { } implementationAttribute)
      {
        factory.DuplicateActorImplementationAttribute(implementationAttribute);
      }

      specifiedActor = genericImplementationAttribute.AttributeClass!.TypeArguments[0];
    }
    else
    {
      specifiedActor = (ITypeSymbol)ImplementationAttribute!.ConstructorArguments[0].Value!;
    }

    return specifiedActor;
  }
}
