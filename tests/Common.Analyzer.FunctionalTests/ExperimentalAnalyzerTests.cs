using CodeArchitects.Platform.Common.Analyzer.Fixtures;
using CodeArchitects.Platform.Common.CodeAnalysis;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Reflection;

namespace CodeArchitects.Platform.Common.Analyzer;

public class ExperimentalAnalyzerTests : AnalyzerTest<ExperimentalAnalyzer>
{
  protected override IEnumerable<Type> ReferencedAssemblyMarkers => new Type[]
  {
    typeof(ExperimentalAttribute)
  };

  protected override IEnumerable<Assembly> ReferencedAsseblies => new Assembly[]
  {
    Assembly.Load("System.Runtime")
  };

  [Fact]
  public async Task EmptyCode_ShouldNotTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  public static class Program
  {
    public static void Main(string[] args)
    {
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().NotContain(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalClassDeclaration_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public class ExperimentalClass { }

  public static class Program
  {
    public static void Main(string[] args)
    {
      ExperimentalClass experimentalClass = default;
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalClassInheritance_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public class ExperimentalClass { }

  public class Derived : ExperimentalClass { }

  public static class Program
  {
    public static void Main(string[] args)
    {
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalGenericClassDeclaration_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public class ExperimentalClass<T> { }

  public static class Program
  {
    public static void Main(string[] args)
    {
      ExperimentalClass<int> experimentalClass = default;
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalGenericClassInheritance_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public class ExperimentalClass<T> { }

  public class Derived : ExperimentalClass<int> { }

  public static class Program
  {
    public static void Main(string[] args)
    {
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task DeclarationAndConstructor_ShouldTriggerSingleCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public class ExperimentalClass { }

  public static class Program
  {
    public static void Main(string[] args)
    {
      ExperimentalClass experimentalClass = new ExperimentalClass();
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalEnumDeclaration_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public enum ExperimentalEnum { }

  public static class Program
  {
    public static void Main(string[] args)
    {
      ExperimentalEnum experimentalEnum = default;
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalStructDeclaration_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public struct ExperimentalStruct { }

  public static class Program
  {
    public static void Main(string[] args)
    {
      ExperimentalStruct experimentalStruct = default;
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalGenericStructDeclaration_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public struct ExperimentalStruct<T> { }

  public static class Program
  {
    public static void Main(string[] args)
    {
      ExperimentalStruct<int> experimentalStruct = default;
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalMethod_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  public class Class
  {
    [Experimental]
    public void ExperimentalMethod()
    {
    }
  }

  public static class Program
  {
    public static void Main(string[] args)
    {
      Class @class = new Class();
      @class.ExperimentalMethod();
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalConstructor_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  public class Class
  {
    public Class(int arg)
    {
    }

    [Experimental]
    public Class(string arg)
    {
    }
  }

  public static class Program
  {
    public static void Main(string[] args)
    {
      Class @class = new Class("""");
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task NonExperimentalConstructor_ShouldNotTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  public class Class
  {
    public Class(int arg)
    {
    }

    [Experimental]
    public Class(string arg)
    {
    }
  }

  public static class Program
  {
    public static void Main(string[] args)
    {
      Class @class = new Class(1);
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().NotContain(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalDelegate_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public delegate void ExperimentalDelegate();

  public static class Program
  {
    public static void Main(string[] args)
    {
      ExperimentalDelegate @delegate = () => { };
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalParameterDeclaration_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public class ExperimentalClass { }

  public static class Program
  {
    public static void Main(string[] args)
    {
    }

    public static void Method(ExperimentalClass param)
    {
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }

  [Fact]
  public async Task ExperimentalGenericParameterDeclaration_ShouldTriggerCAEP000()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.Common.CodeAnalysis;

namespace Test
{
  [Experimental]
  public class ExperimentalClass<T> { }

  public static class Program
  {
    public static void Main(string[] args)
    {
    }

    public static void Method(ExperimentalClass<int> param)
    {
    }
  }
}
";

    // Act
    ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

    // Assert
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAEP000);
  }
}
