using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Actors.Analyzer;

public partial class ActorGeneratorTests
{
  public class CAEPACTR300ValidDictionaryDataAttribute : DataAttribute
  {
    private static string[] s_dictionaryTypes = new[]
    {
      "IDictionary",
      "IReadOnlyDictionary",
      "Dictionary",
      "SortedDictionary",
      "KeyValuePair"
    };

    private static string[] s_keyTypes = new[]
    {
      "bool",
      "byte",
      "System.DateTime",
      "System.DateTimeOffset",
      "decimal",
      "double",
      "System.Guid",
      "short",
      "int",
      "long",
      "object",
      "sbyte",
      "float",
      "string",
      "ushort",
      "uint",
      "ulong"
    };

    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      foreach (string dictionaryType in s_dictionaryTypes)
      {
        foreach (string keyType in s_keyTypes)
        {
          yield return new object?[] { dictionaryType, keyType };
        }
      }
    }
  }

  private class CAEPACTR603WrongSignatureDataAttribute : DataAttribute
  {
    private static string[] s_createAsyncReturnTypes = new[]
    {
      "ValueTask<IMyActor>",
      "object",
      "ValueTask<object>"
    };

    private static string[] s_createAsyncParameters = new[]
    {
      "string id, int state, CancellationToken cancellationToken",
      "int state, CancellationToken cancellationToken",
      "string id, int state",
      "int state",
      "",
      "int id, int state, CancellationToken cancellationToken",
      "string id, int state, string state0",
      "string id, CancellationToken cancellationToken",
      "string id, char state, CancellationToken cancellationToken",
      "string id, int state, string state0, CancellationToken cancellationToken"
    };

    private static string[] s_getReturnTypes = new[]
    {
      "IMyActor",
      "object"
    };

    private static string[] s_getParameters = new[]
    {
      "string id",
      "",
      "int id",
      "string id, int state"
    };

    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      for (int i1 = 0; i1 < s_createAsyncReturnTypes.Length; i1++)
      {
        for (int i2 = 0; i2 < s_createAsyncParameters.Length; i2++)
        {
          for (int i3 = 0; i3 < s_getReturnTypes.Length; i3++)
          {
            for (int i4 = 0; i4 < s_getParameters.Length; i4++)
            {
              if (i1 == 0 && i2 == 0 && i3 == 0 && i4 == 0)
                continue;

              yield return new object?[]
              {
                s_createAsyncReturnTypes[i1],
                s_createAsyncParameters[i2],
                s_getReturnTypes[i3],
                s_getParameters[i4]
              };
            }
          }
        }
      }
    }
  }

  private static void AssertValidFactoryType(Compilation compilation, bool isVirtual, params StateSpec[] stateSpecs)
  {
    INamedTypeSymbol? factoryType = compilation.GetTypeByMetadataName("Actors.Tests.IMyActorFactory");

    INamedTypeSymbol actorFactoryAttributeType = compilation.GetTypeByMetadataName("CodeArchitects.Platform.Actors.ActorFactoryAttribute")!;
    Assert.NotNull(actorFactoryAttributeType);

    INamedTypeSymbol implementationType = compilation.GetTypeByMetadataName("Actors.Tests.MyActor")!;
    Assert.NotNull(implementationType);

    INamedTypeSymbol interfaceType = compilation.GetTypeByMetadataName("Actors.Tests.IMyActor")!;
    Assert.NotNull(interfaceType);

    INamedTypeSymbol taskType = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1")!;
    Assert.NotNull(taskType);

    INamedTypeSymbol stringType = compilation.GetSpecialType(SpecialType.System_String);

    INamedTypeSymbol cancellationTokenType = compilation.GetTypeByMetadataName("System.Threading.CancellationToken")!;
    Assert.NotNull(cancellationTokenType);

    factoryType.Should().NotBeNull();

    factoryType!.GetAttributes().Should().HaveCount(1)
      .And.ContainSingle(attribute =>
        SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, actorFactoryAttributeType) &&
        SymbolEqualityComparer.Default.Equals(attribute.ConstructorArguments[0].Value as INamedTypeSymbol, implementationType));

    ImmutableArray<ISymbol> members = factoryType.GetMembers();
    members.Should().HaveCount(isVirtual ? 1 : 2);

    IMethodSymbol getMethod = members.Should().ContainSingle(member =>
        member.Kind == SymbolKind.Method &&
        member.Name == "Get").Subject.As<IMethodSymbol>();

    getMethod.Parameters.Should().HaveCount(1)
      .And.ContainSingle(parameter =>
        parameter.Name == "id" &&
        SymbolEqualityComparer.Default.Equals(parameter.Type, stringType));

    getMethod.ReturnType.Should().BeEquivalentTo(interfaceType, opt => opt.Using(SymbolEqualityComparer.Default));

    if (!isVirtual)
    {
      IMethodSymbol createAsyncMethod = members.Should().ContainSingle(member =>
          member.Kind == SymbolKind.Method &&
          member.Name == "CreateAsync").Subject.As<IMethodSymbol>();

      createAsyncMethod.Parameters.Should().HaveCount(2 + stateSpecs.Length)
        .And.ContainSingle(parameter =>
          parameter.Name == "id" &&
          SymbolEqualityComparer.Default.Equals(parameter.Type, stringType))
        .And.ContainSingle(parameter =>
          parameter.Name == "cancellationToken" &&
          SymbolEqualityComparer.Default.Equals(parameter.Type, cancellationTokenType));

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

  private static void CompileWithGenerator(string source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics)
  {
    MetadataReference[] references = AppDomain.CurrentDomain
      .GetAssemblies()
      .Where(assembly => !assembly.IsDynamic)
      .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
      .ToArray();

    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, new(languageVersion: LanguageVersion.Preview));
    Compilation originalCompilation = CSharpCompilation.Create(
      assemblyName: "Test",
      syntaxTrees: new[] { syntaxTree },
      references: references,
    options: new(
        outputKind: OutputKind.DynamicallyLinkedLibrary));

    originalCompilation.GetDiagnostics().Where(IsError).Should().BeEmpty();

    GeneratorDriver driver = CSharpGeneratorDriver.Create(
      generators: new[] { new ActorGenerator().AsSourceGenerator() },
      parseOptions: new(languageVersion: LanguageVersion.Preview));
    driver.RunGeneratorsAndUpdateCompilation(originalCompilation, out compilation, out diagnostics);

    compilation.GetDiagnostics().Where(IsError).Should().BeEmpty();
  }

  private static bool IsError(Diagnostic diagnostic)
  {
    return diagnostic.Severity is DiagnosticSeverity.Error;
  }

  private static Expression<Func<Diagnostic, bool>> MatchDiagnostic(string id, int line, int character, string code)
  {
    return diagnostic => MatchDiagnostic(diagnostic, id, line, character, code);
  }

  private static bool MatchDiagnostic(Diagnostic diagnostic, string id, int line, int character, string code)
  {
    if (diagnostic.Id != id)
      return false;

    Location location = diagnostic.Location;

    var lineSpan = location.GetLineSpan();

    if (lineSpan.StartLinePosition.Line + 1 != line)
      return false;

    if (lineSpan.StartLinePosition.Character + 1 != character)
      return false;

    if (location.SourceTree is not { } sourceTree)
      return false;

    string actualCode = sourceTree.ToString().Substring(location.SourceSpan.Start, location.SourceSpan.Length);

    return code == actualCode;
  }

  private record StateSpec(ITypeSymbol Type, string Name);
}
