using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Descriptors;

internal readonly ref partial struct ActorDescriptorFactory
{
  // Class errors (000-299)

  public void DuplicateActorAttribute(AttributeData attribute)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR000, attribute.GetLocation()));
  }

  public void GenericActorsAreNotSupported(INamedTypeSymbol implementationType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR001, implementationType.Locations[0], implementationType.Name));
  }

  public void MissingActorInterface(INamedTypeSymbol implementationType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR002, implementationType.Locations[0], implementationType.Name));
  }

  public void AmbiguousActorInterface(INamedTypeSymbol implementationType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR003, implementationType.Locations[0], implementationType.Name));
  }

  public void InterfaceNotImplemented(INamedTypeSymbol implementationType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR004, implementationType.Locations[0], implementationType.Name));
  }

  public void InterfaceTypeIsNotAnInterface(AttributeData attribute, ITypeSymbol interfaceType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR005, attribute.GetLocation(), interfaceType.Name));
  }

  public void PropertiesAreNotSupported(INamedTypeSymbol implementationType, INamedTypeSymbol interfaceType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR006, implementationType.Locations[0], interfaceType.Name));
  }

  public void EventsAreNotSupported(INamedTypeSymbol implementationType, INamedTypeSymbol interfaceType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR007, implementationType.Locations[0], interfaceType.Name));
  }

  public void ActorCannotBeVirtual(INamedTypeSymbol implementationType, AttributeData attribute)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR008, attribute.GetLocation(), implementationType.Name));
  }

  public void DuplicateActorImplementationAttribute(AttributeData attribute)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR009, attribute.GetLocation()));
  }

  public void MultipleDefaultImplementations(INamedTypeSymbol actorType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR010, actorType.Locations[0], actorType.Name));
  }

  public void InvalidImplementation(INamedTypeSymbol implementationType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR011, implementationType.Locations[0], implementationType.Name));
  }

  public void AbstractImplementation(INamedTypeSymbol implementationType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR012, implementationType.Locations[0], implementationType.Name));
  }


  // State/id errors (300-399)

  public void InvalidStateType(IFieldSymbol stateField, AttributeData stateAttribute)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR300, stateAttribute.GetLocation(), stateField.Type.Name));
  }

  public void InvalidDefaultValue(IFieldSymbol stateField, AttributeData stateAttribute)
  {
    if (_disableDiagnostics)
      return;

    ISymbol stateMember = stateField.AssociatedSymbol ?? stateField;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR301, stateAttribute.GetLocation(), stateMember.Name));
  }

  public void StateMustBeDefinedInBaseActor(INamedTypeSymbol implementationType, AttributeData stateAttribute)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR302, stateAttribute.GetLocation(), implementationType.Name));
  }

  public void AmbiguousActorIdSource(INamedTypeSymbol actorType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR303, actorType.Locations[0]));
  }

  public void InvalidIdType(ISymbol member, ITypeSymbol memberType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR304, member.Locations[0], memberType.Name));
  }

  public void InvalidIdType(AttributeData idTypeAttribute, ITypeSymbol actorIdType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR304, idTypeAttribute.GetLocation(), actorIdType.Name));
  }

  public void MultipleIdSourceInterfaces(ITypeSymbol idType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR305, idType.Locations[0], idType.Name));
  }

  public void InvalidIdSource(ISymbol member, ITypeSymbol sourceIdType, ITypeSymbol actorIdType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR306, member.Locations[0], sourceIdType.Name, actorIdType.Name));
  }

  public void DuplicateActorIdTypeAttribute(AttributeData attribute, INamedTypeSymbol actorType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR307, attribute.GetLocation(), actorType.Name));
  }


  // Method or constructor errors (400-599)

  public void StateComponentNameMismatch(IMethodSymbol constructor)
  {
    if (_disableDiagnostics)
      return;

    Location location = constructor.IsImplicitlyDeclared
      ? constructor.ContainingType.Locations[0]
      : constructor.Locations[0];

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR400, location));
  }

  public void AmbiguousActorConstructor(INamedTypeSymbol implementationType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR401, implementationType.Locations[0]));
  }

  public void WrongGenericActorContext(IParameterSymbol parameter, INamedTypeSymbol actorType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR402, parameter.Locations[0], parameter.Name, actorType.Name));
  }

  public void GenericMethodsAreNotSupported(IMethodSymbol method)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR403, method.Locations[0], method.Name));
  }

  public void InvalidMethodReturnType(IMethodSymbol method)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR404, method.Locations[0], method.Name, method.ReturnType.Name));
  }

  public void CancellationTokenMustBeLastParameter(IMethodSymbol method, IParameterSymbol parameter)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR405, parameter.Locations[0], method.Name));
  }


  // Factory errors (600-699)

  public void DuplicateActorFactoryAttribute(AttributeData attribute)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR600, attribute.GetLocation()));
  }

  public void GenericActorFactoriesAreNotSupported(INamedTypeSymbol factoryType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR601, factoryType.Locations[0], factoryType.Name));
  }

  public void MissingActorFactoryType(INamedTypeSymbol actorType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR602, actorType.Locations[0], actorType.Name));
  }

  public void InvalidActorFactoryType(INamedTypeSymbol factoryType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR603, factoryType.Locations[0], factoryType.Name));
  }

  public void AmbiguousActorFactoryType(INamedTypeSymbol actorType, INamedTypeSymbol factoryType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR604, factoryType.Locations[0], actorType.Name));
  }


  // Other errors (700-999)

  public void InvalidActorMessage(INamedTypeSymbol actorType, ITypeSymbol messageType)
  {
    if (_disableDiagnostics)
      return;

    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.CAEPACTR700, actorType.Locations[0], messageType.Name));
  }
}
