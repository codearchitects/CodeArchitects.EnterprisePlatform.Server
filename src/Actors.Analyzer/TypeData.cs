using CodeArchitects.Platform.Actors.Analyzer.Descriptors;
using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer;

internal record TypeData(INamedTypeSymbol Type);

internal record ActorData(
  INamedTypeSymbol Type,
  AttributeData? ActorAttribute,
  AttributeData? GenericActorAttribute,
  AttributeData? VirtualAttribute,
  AttributeData? IdTypeAttribute,
  AttributeData? GenericIdTypeAttribute) : TypeData(Type)
{
  public INamedTypeSymbol? GetSpecifiedInterfaceType(in ActorDescriptorFactory factory)
  {
    ITypeSymbol? specifiedInterface;
    AttributeData attribute;

    if (GenericActorAttribute is not null)
    {
      if (ActorAttribute is not null)
      {
        factory.DuplicateActorAttribute(ActorAttribute);
      }

      attribute = GenericActorAttribute;
      specifiedInterface = GetFromGenericAttribute(GenericActorAttribute);
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

  public ITypeSymbol? GetSpecifiedIdType(in ActorDescriptorFactory factory)
  {
    ITypeSymbol? specifiedIdType = null;

    if (GenericIdTypeAttribute is not null)
    {
      if (IdTypeAttribute is not null)
      {
        factory.DuplicateActorIdTypeAttribute(IdTypeAttribute, Type);
      }

      specifiedIdType = GenericIdTypeAttribute.AttributeClass!.TypeArguments[0];
    }
    else if (IdTypeAttribute is not null)
    {
      specifiedIdType = (ITypeSymbol)IdTypeAttribute!.ConstructorArguments[0].Value!;
    }

    return specifiedIdType;
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

    if (GenericFactoryAttribute is not null)
    {
      if (FactoryAttribute is not null)
      {
        factory.DuplicateActorFactoryAttribute(FactoryAttribute);
      }

      specifiedActor = GenericFactoryAttribute.AttributeClass!.TypeArguments[0];
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

    if (GenericImplementationAttribute is not null)
    {
      if (ImplementationAttribute is not null)
      {
        factory.DuplicateActorImplementationAttribute(ImplementationAttribute);
      }

      specifiedActor = GenericImplementationAttribute.AttributeClass!.TypeArguments[0];
    }
    else
    {
      specifiedActor = (ITypeSymbol)ImplementationAttribute!.ConstructorArguments[0].Value!;
    }

    return specifiedActor;
  }
}
