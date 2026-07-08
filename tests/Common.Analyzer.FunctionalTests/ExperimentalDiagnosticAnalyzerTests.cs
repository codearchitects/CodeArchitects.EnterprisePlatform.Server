using Verify = CodeArchitects.Platform.Common.Analyzer.Fixtures.CommonAnalyzerVerifier<CodeArchitects.Platform.Common.Analyzer.ExperimentalDiagnosticAnalyzer>;

namespace CodeArchitects.Platform.Common.Analyzer;

public class ExperimentalDiagnosticAnalyzerTests
{
  [Fact]
  public async Task ExperimentalClassDeclaration_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      [Experimental]
      public class ExperimentalClass { }
      
      public class C
      {
        public void M()
        {
          ExperimentalClass? experimentalClass = default;
        }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(12, 5, 12, 22);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalClassInheritance_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      [Experimental]
      public class ExperimentalClass { }
      
      public class Derived : ExperimentalClass { }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(8, 24, 8, 41);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalGenericClassDeclaration_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;

      [Experimental]
      public class ExperimentalClass<T> { }
      
      public class C
      {
        public void M()
        {
          ExperimentalClass<int>? experimentalClass = default;
        }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(12, 5, 12, 27);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalGenericClassInheritance_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      [Experimental]
      public class ExperimentalClass<T> { }
      
      public class Derived : ExperimentalClass<int> { }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(8, 24, 8, 46);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task DeclarationAndConstructor_ShouldTriggerSingleCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;

      [Experimental]
      public class ExperimentalClass { }
      
      public class C
      {
        public void M()
        {
          ExperimentalClass? experimentalClass = new ExperimentalClass();
        }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(12, 5, 12, 22);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalEnumDeclaration_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;

      [Experimental]
      public enum ExperimentalEnum { }
      
      public class C
      {
        public void M()
        {
          ExperimentalEnum experimentalEnum = default;
        }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(12, 5, 12, 21);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalStructDeclaration_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      [Experimental]
      public struct ExperimentalStruct { }
      
      public class C
      {
        public void M()
        {
          ExperimentalStruct experimentalStruct = default;
        }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(12, 5, 12, 23);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalGenericStructDeclaration_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      [Experimental]
      public struct ExperimentalStruct<T> { }
      
      public class C
      {
        public void M()
        {
          ExperimentalStruct<int> experimentalStruct = default;
        }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(12, 5, 12, 28);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalMethod_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      public class Class
      {
        [Experimental]
        public void ExperimentalMethod() {}
      }
      
      public class C
      {
        public void M()
        {
          Class @class = new Class();
          @class.ExperimentalMethod();
        }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(16, 12, 16, 30);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalConstructor_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      public class Class
      {
        public Class(int arg) { }
      
        [Experimental]
        public Class(string arg) { }
      }
      
      public class C
      {
        public void M()
        {
          Class @class = new Class("");
        }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(17, 20, 17, 33);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task NonExperimentalConstructor_ShouldNotTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;

      public class Class
      {
        public Class(int arg) { }
      
        [Experimental]
        public Class(string arg) { }
      }
      
      public class C
      {
        public void M()
        {
          Class @class = new Class(1);
        }
      }
      """;

    await Verify.VerifyAnalyzerAsync(source);
  }

  [Fact]
  public async Task ExperimentalDelegate_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      [Experimental]
      public delegate void ExperimentalDelegate();
      
      public class C
      {
        public void M()
        {
          ExperimentalDelegate @delegate = () => { };
        }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(12, 5, 12, 25);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalParameterDeclaration_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      [Experimental]
      public class ExperimentalClass { }
      
      public class C
      {
        public void M() { }
      
        public void Method(ExperimentalClass param) { }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(12, 22, 12, 39);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }

  [Fact]
  public async Task ExperimentalGenericParameterDeclaration_ShouldTriggerCAEP000()
  {
    const string source = """
      using CodeArchitects.Platform.Common.CodeAnalysis;
      
      namespace Common.Tests;
      
      [Experimental]
      public class ExperimentalClass<T> { }
      
      public class C
      {
        public void M() { }
      
        public void Method(ExperimentalClass<int> param) { }
      }
      """;

    var expected = Verify.Diagnostic(DiagnosticIds.CAEP000).WithSpan(12, 22, 12, 44);

    await Verify.VerifyAnalyzerAsync(source, expected);
  }
}
