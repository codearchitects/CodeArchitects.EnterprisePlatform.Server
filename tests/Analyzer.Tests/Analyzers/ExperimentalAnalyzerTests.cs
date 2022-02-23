using CodeArchitects.Platform.Analyzer.Analyzers;
using CodeArchitects.Platform.CodeAnalysis;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace CodeArchitects.Platform.Analyzer.Tests.Analyzers;

public class ExperimentalAnalyzerTests : AnalyzerTest
{
  protected override Type AnalyzerType => typeof(ExperimentalAnalyzer);

  protected override IEnumerable<Type> ReferencedAssemblyMarkers => new Type[]
  {
      typeof(ExperimentalAttribute)
  };

  protected override IEnumerable<Assembly> ReferencedAsseblies => new Assembly[]
  {
      Assembly.Load("System.Runtime")
  };

  [Fact]
  public async Task EmptyCode_ShouldNotTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().NotContain(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalClassDeclaration_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalClassInheritance_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalGenericClassDeclaration_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalGenericClassInheritance_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task DeclarationAndConstructor_ShouldTriggerSingleCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalEnumDeclaration_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalStructDeclaration_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalGenericStructDeclaration_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalMethod_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalConstructor_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task NonExperimentalConstructor_ShouldNotTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().NotContain(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalDelegate_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalParameterDeclaration_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }

  [Fact]
  public async Task ExperimentalGenericParameterDeclaration_ShouldTriggerCAESP001()
  {
    // Arrange
    const string code = @"
using CodeArchitects.Platform.CodeAnalysis;

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
    diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP001);
  }
}
