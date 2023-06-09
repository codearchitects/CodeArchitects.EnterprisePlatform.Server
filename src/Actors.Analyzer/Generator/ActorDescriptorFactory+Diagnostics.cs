using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CodeArchitects.Platform.Actors.Analyzer.Generator;

internal readonly ref partial struct ActorDescriptorFactory
{
  // Class errors (000-299)

  private void GenericActorsAreNotSupported(INamedTypeSymbol implementationType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR000, implementationType.Locations[0], implementationType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void MissingActorInterface(INamedTypeSymbol actorType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR001, actorType.Locations[0], actorType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void AmbiguousActorInterface(INamedTypeSymbol implementationType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR002, implementationType.Locations[0], implementationType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void InterfaceNotImplemented(INamedTypeSymbol implementationType, AttributeData attribute)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR003, GetTargetLocation(attribute), implementationType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void InvalidInterfaceType(AttributeData attribute, ITypeSymbol interfaceType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR004, attribute.GetLocation(), interfaceType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void PropertiesAreNotSupported(INamedTypeSymbol implementationType, INamedTypeSymbol interfaceType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR005, implementationType.Locations[0], interfaceType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void EventsAreNotSupported(INamedTypeSymbol implementationType, INamedTypeSymbol interfaceType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR006, implementationType.Locations[0], interfaceType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void ActorCannotBeVirtual(INamedTypeSymbol implementationType, AttributeData attribute)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR007, attribute.GetLocation(), implementationType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void MultipleDefaultImplementations(INamedTypeSymbol actorType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR008, actorType.Locations[0], actorType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void ActorNotInherited(INamedTypeSymbol implementationType, AttributeData attribute)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR009, GetTargetLocation(attribute), implementationType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void AbstractImplementation(INamedTypeSymbol implementationType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR010, implementationType.Locations[0], implementationType.Name);
    _diagnostics.Add(diagnostic);
  }


  // State/id errors (300-399)

  private void InvalidStateType(IFieldSymbol stateField, AttributeData stateAttribute)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR300, stateAttribute.GetLocation(), stateField.Type.ToDisplayString(Format.Name));
    _diagnostics.Add(diagnostic);
  }

  private void InvalidDefaultValue(IFieldSymbol stateField, AttributeData stateAttribute)
  {
    if (_disableActorDiagnostics)
      return;

    ISymbol stateMember = stateField.AssociatedSymbol ?? stateField;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR301, stateAttribute.GetLocation(), stateMember.Name);
    _diagnostics.Add(diagnostic);
  }

  private void StateMustBeDefinedInBaseActor(INamedTypeSymbol implementationType, AttributeData stateAttribute)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR302, stateAttribute.GetLocation(), implementationType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void MultipleIdMembers(INamedTypeSymbol actorType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR303, actorType.Locations[0]);
    _diagnostics.Add(diagnostic);
  }

  private void InvalidIdType(ISymbol member, ITypeSymbol memberType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR304, member.Locations[0], memberType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void InvalidIdType(AttributeData idTypeAttribute, ITypeSymbol actorIdType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR304, idTypeAttribute.GetLocation(), actorIdType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void InvalidIdMember(ISymbol member, ITypeSymbol memberIdType, ITypeSymbol actorIdType, AttributeData? idTypeAttribute, ISymbol? idMember)
  {
    if (_disableActorDiagnostics)
      return;

    string? filePath = null;
    TextSpan textSpan = default;

    if (idTypeAttribute is not null)
    {
      if (idTypeAttribute.ApplicationSyntaxReference is SyntaxReference syntaxReference)
      {
        filePath = syntaxReference.SyntaxTree.FilePath;
        textSpan = syntaxReference.Span;
      }
    }
    else if (idMember is not null)
    {
      Location memberLocation = idMember.Locations[0];
      filePath = memberLocation.SourceTree?.FilePath;
      textSpan = memberLocation.SourceSpan;
    }

    string[] properties = filePath is null
      ? Array.Empty<string>()
      : new[] { filePath, textSpan.Start.ToString(), textSpan.Length.ToString() };

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR305, member.Locations[0], properties, memberIdType.ToDisplayString(Format.Name), actorIdType.ToDisplayString(Format.Name));
    _diagnostics.Add(diagnostic);
  }

  private void NullDefaultValue(IFieldSymbol stateField, AttributeData stateAttribute)
  {
    if (_disableActorDiagnostics)
      return;

    ISymbol stateMember = stateField.AssociatedSymbol ?? stateField;

    DiagnosticSeverity? severity = _treatNullableAsError
      ? DiagnosticSeverity.Error
      : null;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR306, severity, stateAttribute.GetLocation(), stateMember.Name);
    _diagnostics.Add(diagnostic);
  }


  // Method or constructor errors (400-599)

  private void StateComponentNameMismatch(IMethodSymbol constructor)
  {
    if (_disableActorDiagnostics)
      return;

    Location location = constructor.IsImplicitlyDeclared
      ? constructor.ContainingType.Locations[0]
      : constructor.Locations[0];

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR400, location);
    _diagnostics.Add(diagnostic);
  }

  private void AmbiguousActorConstructor(INamedTypeSymbol implementationType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR401, implementationType.Locations[0]);
    _diagnostics.Add(diagnostic);
  }

  private void WrongGenericActorContext(IParameterSymbol parameter, INamedTypeSymbol actorType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR402, parameter.Locations[0], parameter.Name, actorType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void GenericMethodsAreNotSupported(IMethodSymbol method)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR403, method.Locations[0], method.Name);
    _diagnostics.Add(diagnostic);
  }

  private void InvalidMethodReturnType(IMethodSymbol method)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR404, method.Locations[0], method.Name, method.ReturnType.ToDisplayString(Format.Name));
    _diagnostics.Add(diagnostic);
  }

  private void CancellationTokenMustBeLastParameter(IMethodSymbol method, IParameterSymbol parameter)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR405, parameter.Locations[0], method.Name);
    _diagnostics.Add(diagnostic);
  }


  // Factory errors (600-699)

  private void GenericActorFactoriesAreNotSupported(INamedTypeSymbol factoryType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR600, factoryType.Locations[0], factoryType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void MissingActorFactoryType(INamedTypeSymbol actorType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR601, actorType.Locations[0], actorType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void InvalidActorFactoryType(INamedTypeSymbol factoryType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR602, factoryType.Locations[0], factoryType.Name);
    _diagnostics.Add(diagnostic);
  }

  private void AmbiguousActorFactoryType(INamedTypeSymbol actorType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR603, actorType.Locations[0], actorType.Name);
    _diagnostics.Add(diagnostic);
  }


  // Other errors (700-999)

  private void DuplicateAttribute(AttributeData attribute)
  {
    if (_disableActorDiagnostics)
      return;

    string attributeName = attribute.AttributeClass!.Name;
    int suffixIndex = attributeName.LastIndexOf("Attribute");

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR700, attribute.GetLocation(), attributeName.Remove(suffixIndex));
    _diagnostics.Add(diagnostic);
  }

  private void TargetIsNotAnActor(INamedTypeSymbol type, ITypeSymbol targetType)
  {
    if (_disableActorDiagnostics)
      return;

    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR701, type.Locations[0], targetType.ToDisplayString(Format.Name));
    _diagnostics.Add(diagnostic);
  }

  private void InvalidActorMessage(ActorData actor, ITypeSymbol messageHandlerType, ITypeSymbol messageType, ITypeSymbol? idType)
  {
    if (_disableActorDiagnostics)
      return;

    Location location = actor.GetBaseTypeLocation(messageHandlerType);
    string[] properties = new[] { idType?.ToDisplayString(Format.FullName) ?? "System.String" };
    DiagnosticReference diagnostic = DiagnosticReference.Create(DiagnosticIds.CAEPACTR702, location, properties, messageType.Name);
    _diagnostics.Add(diagnostic);
  }


  private static Location GetTargetLocation(AttributeData attribute)
  {
    if (attribute.ApplicationSyntaxReference is not SyntaxReference syntaxReference)
      return Location.None;

    AttributeSyntax attributeSyntax = (AttributeSyntax)syntaxReference.GetSyntax();

    return attributeSyntax.GetTarget(attributeSyntax.Name)?.GetLocation() ??
      Location.Create(attributeSyntax.SyntaxTree, attributeSyntax.Span);
  }
}
