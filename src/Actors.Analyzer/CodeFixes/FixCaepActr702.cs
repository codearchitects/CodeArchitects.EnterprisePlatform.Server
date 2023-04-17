using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal sealed class FixCaepActr702 : FixingActionProvider<SimpleBaseTypeSyntax>
{
  private const string s_title = "Make '{0}' implement '{1}'";

  protected override string DiagnosticId => CAEPACTR702;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, SimpleBaseTypeSyntax baseType, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    if (!document.SupportsSemanticModel)
      return None;

    if (properties.Count != 1)
      return Fail("Expected an id type.");

    return new(GetFixingActionCoreAsync(document, baseType, properties[0], cancellationToken));
  }

  private static async Task<CodeAction?> GetFixingActionCoreAsync(Document document, SimpleBaseTypeSyntax baseType, string idTypeFullName, CancellationToken cancellationToken)
  {
    Solution solution = document.Project.Solution;
    
    SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
    if (semanticModel is null)
      return null;

    Compilation compilation = semanticModel.Compilation;
    INamedTypeSymbol? actorMessageTypeSymbol = compilation.GetTypeByMetadataName("CodeArchitects.Platform.Actors.Messaging.IActorMessage`1");
    if (actorMessageTypeSymbol is null)
      return null;

    TypeInfo typeInfo = semanticModel.GetTypeInfo(baseType.Type, cancellationToken);
    if (typeInfo.Type is not INamedTypeSymbol { TypeArguments: [INamedTypeSymbol messageTypeSymbol] })
      return null;

    INamedTypeSymbol? idTypeSymbol = compilation.GetTypeByMetadataName(idTypeFullName);
    if (idTypeSymbol is null)
      return null;

    MessageTypeInfo? messageTypeInfo = await GetMessageTypeInfoAsync(solution, messageTypeSymbol, actorMessageTypeSymbol, cancellationToken).ConfigureAwait(false);
    if (messageTypeInfo is null)
      return null;

    return CodeAction.Create(
      title: string.Format(s_title, messageTypeSymbol.ToDisplayString(Format.Name), idTypeSymbol.ToDisplayString(Format.Name)),
      createChangedDocument: cancellationToken => Task.FromResult(ImplementActorMessageInterface(messageTypeInfo, actorMessageTypeSymbol, idTypeSymbol)),
      equivalenceKey: CAEPACTR702);
  }

  private static Document ImplementActorMessageInterface(MessageTypeInfo info, INamedTypeSymbol actorMessageTypeSymbol, INamedTypeSymbol idTypeSymbol)
  {
    (Document document, SemanticModel semanticModel, CompilationUnitSyntax root, ClassDeclarationSyntax @class, BaseTypeSyntax? baseType) = info;

    bool isActorMessageTypeAccessible = actorMessageTypeSymbol.IsAccessible(semanticModel, @class.SpanStart);
    bool isIdTypeAccessible = idTypeSymbol.IsAccessible(semanticModel, @class.SpanStart);

    GenericNameSyntax newBaseTypeName = CreateNewBaseTypeName(idTypeSymbol);

    root = baseType is null
      ? AddActorMessageInterface(root, info.Class, newBaseTypeName)
      : ChangeActorMessageInterface(root, baseType, newBaseTypeName);

    if (!isActorMessageTypeAccessible)
    {
      root = root.AddUsings(SyntaxFactory.UsingDirective(
        usingKeyword: SyntaxFactory.Token(
          leading: default,
          kind: SyntaxKind.UsingKeyword,
          trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
        staticKeyword: default,
        alias: null,
        name: SyntaxFactory.QualifiedName(
          left: SyntaxFactory.QualifiedName(
            left: SyntaxFactory.QualifiedName(
              left: SyntaxFactory.IdentifierName("CodeArchitects"),
              right: SyntaxFactory.IdentifierName("Platform")),
            right: SyntaxFactory.IdentifierName("Actors")),
          right: SyntaxFactory.IdentifierName("Messaging")),
        semicolonToken: SyntaxFactory.Token(
          leading: default,
          kind: SyntaxKind.SemicolonToken,
          trailing: SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed))));
    }

    if (!isIdTypeAccessible)
    {
      List<INamespaceSymbol> namespaces = new();
      INamespaceSymbol? @namespace = idTypeSymbol.ContainingNamespace;
      while (@namespace is not null && !@namespace.IsGlobalNamespace)
      {
        namespaces.Add(@namespace);
        @namespace = @namespace.ContainingNamespace;
      }

      NameSyntax name = SyntaxFactory.IdentifierName(namespaces[^1].Name);
      for (int i = namespaces.Count - 2; i >= 0; i--)
      {
        name = SyntaxFactory.QualifiedName(
          left: name,
          right: SyntaxFactory.IdentifierName(namespaces[i].Name));
      }

      root = root.AddUsings(SyntaxFactory.UsingDirective(
        usingKeyword: SyntaxFactory.Token(
          leading: default,
          kind: SyntaxKind.UsingKeyword,
          trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
        staticKeyword: default,
        alias: null,
        name: name,
        semicolonToken: SyntaxFactory.Token(
          leading: default,
          kind: SyntaxKind.SemicolonToken,
          trailing: SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed))));
    }

    return document.WithSyntaxRoot(root);
  }

  private static CompilationUnitSyntax ChangeActorMessageInterface(CompilationUnitSyntax root, BaseTypeSyntax baseType, GenericNameSyntax newBaseTypeName)
  {
    return root.ReplaceNode(
      oldNode: baseType,
      newNode: baseType.WithType(newBaseTypeName.WithTriviaFrom(baseType.Type)));
  }

  private static CompilationUnitSyntax AddActorMessageInterface(CompilationUnitSyntax root, ClassDeclarationSyntax @class, GenericNameSyntax newBaseTypeName)
  {
    return root.ReplaceNode(
      oldNode: @class,
      newNode: AddToBaseList(@class, newBaseTypeName));
  }

  private static GenericNameSyntax CreateNewBaseTypeName(INamedTypeSymbol idTypeSymbol)
  {
    TypeSyntax typeArgument = idTypeSymbol.SpecialType switch
    {
      SpecialType.System_Object  => MakePredefinedType(SyntaxKind.ObjectKeyword),
      SpecialType.System_Boolean => MakePredefinedType(SyntaxKind.BoolKeyword),
      SpecialType.System_Char    => MakePredefinedType(SyntaxKind.CharKeyword),
      SpecialType.System_SByte   => MakePredefinedType(SyntaxKind.SByteKeyword),
      SpecialType.System_Byte    => MakePredefinedType(SyntaxKind.ByteKeyword),
      SpecialType.System_Int16   => MakePredefinedType(SyntaxKind.ShortKeyword),
      SpecialType.System_UInt16  => MakePredefinedType(SyntaxKind.UShortKeyword),
      SpecialType.System_Int32   => MakePredefinedType(SyntaxKind.IntKeyword),
      SpecialType.System_UInt32  => MakePredefinedType(SyntaxKind.UIntKeyword),
      SpecialType.System_Int64   => MakePredefinedType(SyntaxKind.LongKeyword),
      SpecialType.System_UInt64  => MakePredefinedType(SyntaxKind.ULongKeyword),
      SpecialType.System_Decimal => MakePredefinedType(SyntaxKind.DecimalKeyword),
      SpecialType.System_Single  => MakePredefinedType(SyntaxKind.FloatKeyword),
      SpecialType.System_Double  => MakePredefinedType(SyntaxKind.DoubleKeyword),
      SpecialType.System_String  => MakePredefinedType(SyntaxKind.StringKeyword),
      _                          => SyntaxFactory.IdentifierName(idTypeSymbol.ToDisplayString(Format.Name))
    };

    return SyntaxFactory.GenericName(
      identifier: SyntaxFactory.Identifier("IActorMessage"),
      typeArgumentList: SyntaxFactory.TypeArgumentList(
        arguments: SyntaxFactory.SingletonSeparatedList(typeArgument)));

    static PredefinedTypeSyntax MakePredefinedType(SyntaxKind kind) => SyntaxFactory.PredefinedType(SyntaxFactory.Token(kind));
  }

  private static async Task<MessageTypeInfo?> GetMessageTypeInfoAsync(Solution solution, INamedTypeSymbol messageTypeSymbol, INamedTypeSymbol actorMessageTypeSymbol, CancellationToken cancellationToken)
  {
    MessageTypeInfo? result = null;
    foreach (Location location in messageTypeSymbol.Locations)
    {
      Document? document = solution.GetDocument(location.SourceTree);
      if (document is null || !document.SupportsSemanticModel)
        continue;

      if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not CompilationUnitSyntax root)
        continue;

      SyntaxNode node = root.FindNode(location.SourceSpan);
      if (node is not ClassDeclarationSyntax @class)
        continue;

      SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
      if (semanticModel is null)
        continue;

      result = new(document, semanticModel, root, @class, null);

      if (@class.BaseList is not BaseListSyntax baseList)
        continue;

      foreach (BaseTypeSyntax baseType in baseList.Types)
      {
        TypeInfo messageBaseTypeInfo = semanticModel.GetTypeInfo(baseType.Type);
        if (messageBaseTypeInfo.Type is not INamedTypeSymbol { TypeArguments.Length: 1 } messageBaseTypeSymbol)
          continue;

        if (SymbolEqualityComparer.Default.Equals(messageBaseTypeSymbol.OriginalDefinition, actorMessageTypeSymbol))
        {
          return result with { BaseType = baseType };
        }
      }
    }

    return result;
  }

  private record MessageTypeInfo(
    Document Document,
    SemanticModel SemanticModel,
    CompilationUnitSyntax Root,
    ClassDeclarationSyntax Class,
    BaseTypeSyntax? BaseType);
}
