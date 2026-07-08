using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

namespace CodeArchitects.Platform.Actors.Analyzer;

public partial class ActorGeneratorTests
{
  private static void AssertValidFactoryType(Compilation compilation, bool isVirtual, bool getsIdFromState, string? idTypeFullName, params StateSpec[] stateSpecs)
  {
    INamedTypeSymbol? factoryType = compilation.GetTypeByMetadataName("Actors.Tests.IMyActorFactory");

    INamedTypeSymbol actorFactoryAttributeType = compilation.GetTypeByMetadataName("CodeArchitects.Platform.Actors.ActorFactoryAttribute")!;
    actorFactoryAttributeType.Should().NotBeNull();

    INamedTypeSymbol implementationType = compilation.GetTypeByMetadataName("Actors.Tests.MyActor")!;
    implementationType.Should().NotBeNull();

    ITypeSymbol interfaceType = implementationType.Interfaces.Single(type => type.Name.Contains("IMyActor"));

    INamedTypeSymbol taskType = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1")!;
    taskType.Should().NotBeNull();

    INamedTypeSymbol idType = idTypeFullName is null
      ? compilation.GetSpecialType(SpecialType.System_String)
      : compilation.GetTypeByMetadataName(idTypeFullName) ?? throw new Exception($"Invalid id type: {idTypeFullName}");

    INamedTypeSymbol cancellationTokenType = compilation.GetTypeByMetadataName("System.Threading.CancellationToken")!;
    cancellationTokenType.Should().NotBeNull();

    factoryType.Should().NotBeNull();

    factoryType!.GetAttributes().Should().HaveCount(1).And.ContainSingle(attribute =>
      SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, actorFactoryAttributeType) &&
      SymbolEqualityComparer.Default.Equals(attribute.ConstructorArguments[0].Value as INamedTypeSymbol, implementationType));

    ImmutableArray<ISymbol> members = factoryType.GetMembers();
    members.Should().HaveCount(isVirtual ? 1 : 2);

    IMethodSymbol getMethod = members.Should().ContainSingle(member =>
      member.Kind == SymbolKind.Method &&
      member.Name == "Get").Subject.As<IMethodSymbol>();

    getMethod.Parameters.Should().HaveCount(1).And.ContainSingle(parameter =>
      parameter.Name == "id" &&
      SymbolEqualityComparer.Default.Equals(parameter.Type, idType));

    getMethod.ReturnType.Should().BeEquivalentTo(interfaceType, opt => opt.Using(SymbolEqualityComparer.Default));

    if (!isVirtual)
    {
      IMethodSymbol createAsyncMethod = members.Should().ContainSingle(member =>
        member.Kind == SymbolKind.Method &&
        member.Name == "CreateAsync").Subject.As<IMethodSymbol>();

      int expectedParameterCount = getsIdFromState
        ? 1 + stateSpecs.Length
        : 2 + stateSpecs.Length;
      createAsyncMethod.Parameters.Should().HaveCount(expectedParameterCount)
        .And.ContainSingle(parameter =>
          parameter.Name == "cancellationToken" &&
          SymbolEqualityComparer.Default.Equals(parameter.Type, cancellationTokenType));

      if (!getsIdFromState)
      {
        createAsyncMethod.Parameters.Should().ContainSingle(parameter =>
          parameter.Name == "id" &&
          SymbolEqualityComparer.Default.Equals(parameter.Type, idType));
      }

      foreach (StateSpec spec in stateSpecs)
      {
        createAsyncMethod.Parameters.Should().ContainSingle(parameter =>
          parameter.Name == spec.Name &&
          SymbolEqualityComparer.Default.Equals(parameter.Type, spec.Type));
      }

      INamedTypeSymbol createAsyncMethodReturnType = createAsyncMethod.ReturnType.Should().BeAssignableTo<INamedTypeSymbol>().Subject;
      createAsyncMethodReturnType.IsGenericType.Should().BeTrue();
      createAsyncMethodReturnType.OriginalDefinition.Should().BeEquivalentTo(taskType, opt => opt.Using(SymbolEqualityComparer.Default));
      createAsyncMethodReturnType.TypeArguments[0].Should().BeEquivalentTo(interfaceType, opt => opt.Using(SymbolEqualityComparer.Default));
    }
  }

  private static Compilation CompileWithGenerator(string source)
  {
    MetadataReference[] references = AppDomain.CurrentDomain
      .GetAssemblies()
      .Where(assembly => !assembly.IsDynamic)
      .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
      .ToArray();

    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(languageVersion: LanguageVersion.Preview));
    Compilation originalCompilation = CSharpCompilation.Create(
      assemblyName: "Test",
      syntaxTrees: new[] { syntaxTree },
      references: references,
      options: new(outputKind: OutputKind.DynamicallyLinkedLibrary));

    IEnumerable<Diagnostic> errors = originalCompilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity is DiagnosticSeverity.Error);
    errors.Should().BeEmpty();

    GeneratorDriver driver = CSharpGeneratorDriver.Create(
      generators: new[] { new ActorSourceGenerator().AsSourceGenerator() },
      parseOptions: new(languageVersion: LanguageVersion.Preview));
    driver.RunGeneratorsAndUpdateCompilation(originalCompilation, out Compilation compilation, out _);

    compilation.GetDiagnostics().Should().BeEmpty();

    return compilation;
  }

  private record StateSpec(ITypeSymbol Type, string Name);
}
