using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CodeArchitects.Platform.Actors.Analyzer;

public partial class ActorGeneratorTests
{
  [Fact]
  public void ShouldGenerateFactory_WhenActorHasImplicitDefaultConstructor()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
    AssertValidFactoryType(compilation, true);
  }

  [Fact]
  public void ShouldGenerateFactory_WhenActorHasParameterlessConstructor()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public MyActor()
          {
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    AssertValidFactoryType(compilation, true);
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldGenerateFactory_WhenActorIsStateless()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyService { }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public MyActor(IMyService service, IActorContext context)
          {
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
    AssertValidFactoryType(compilation, true);
  }

  [Fact]
  public void ShouldGenerateFactory_WhenActorHasOneStateComponent()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyService { }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private int _state0;
          
          public MyActor(int state0, IMyService service, IActorContext context)
          {
            _state0 = state0;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
    AssertValidFactoryType(compilation, false,
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int32), "state0"));
  }

  [Fact]
  public void ShouldGenerateFactory_WhenActorHasMultipleStateComponents()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyService { }

        public interface IMyActor
        {
        }

        [Actor]
        public class MyActor : IMyActor
        {
          [State] private int _state0;
          [State] private short _state1;
      
          public MyActor(int state0, IMyService service, IActorContext context, short state1)
          {
            _state0 = state0;
            _state1 = state1;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
    AssertValidFactoryType(compilation, false,
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int32), "state0"),
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int16), "state1"));
  }

  [Fact]
  public void ShouldGenerateFactory_WhenActorIsVirtual()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyService { }

        public interface IMyActor
        {
        }

        [Actor, Virtual]
        public class MyActor : IMyActor
        {
          [State] private int _state0;
          [State] private short _state1;
      
          public MyActor(int state0, IMyService service, IActorContext context, short state1)
          {
            _state0 = state0;
            _state1 = state1;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
    AssertValidFactoryType(compilation, true,
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int32), "state0"),
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int16), "state1"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR000_WhenClassHasDuplicateActorAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor, Actor<IMyActor>]
        public class MyActor : IMyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR000, 9, 4, "Actor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR001_WhenClassIsGeneric()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor<T> : IMyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR001, 10, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR002_WhenClassDoesNotImplementAnyInterface()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR002, 10, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR003_WhenClassImplementsMoreThanOneInterfaceAndTheActorInterfaceIsNotSpecified()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IOther
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor, IOther
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR003, 14, 16, "MyActor"));
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR003_WhenClassImplementsMoreThanOneInterfaceAndTheActorInterfaceIsSpecifiedWithNonGenericAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IOther
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor(InterfaceType = typeof(IMyActor))]
        public class MyActor : IMyActor, IOther
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR003_WhenClassImplementsMoreThanOneInterfaceAndTheActorInterfaceIsSpecifiedWithGenericAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IOther
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor<IMyActor>]
        public class MyActor : IMyActor, IOther
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldTriggerCAEPACTR004_WhenClassDoesNotImplementTheInterfaceSpecifiedWithNonGenericAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IOther
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor(InterfaceType = typeof(IMyActor))]
        public class MyActor : IOther
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR004, 14, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR004_WhenClassDoesNotImplementTheInterfaceSpecifiedWithGenericAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IOther
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor<IMyActor>]
        public class MyActor : IOther
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR004, 14, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR005_WhenInterfaceTypeSpecifiedWithNonGenericAttributeIsNotAnInterface()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public class Other
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor(InterfaceType = typeof(Other))]
        public class MyActor : IMyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR005, 13, 4, "Actor(InterfaceType = typeof(Other))"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR005_WhenInterfaceTypeSpecifiedWithGenericAttributeIsNotAnInterface()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public class Other
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor<Other>]
        public class MyActor : IMyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR005, 13, 4, "Actor<Other>"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR006_WhenInterfaceHasProperties()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
          int Property { get; }
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public int Property { get; }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR006, 11, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR007_WhenInterfaceHasEvents()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using System;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
          event Action Event;
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public event Action Event;
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR007, 12, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR008_WhenActorHasUninitializableStateAndIsMarkedAsVirtual()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public class MyActorState
        {
          public MyActorState(int arg) { }
        }

        public interface IMyActor
        {
        }
        
        [Actor, Virtual]
        public class MyActor : IMyActor
        {
          [State] private MyActorState _state;

          public MyActor(MyActorState state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR008, 14, 11, "Virtual"));
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR008_WhenActorHasInitializableComplexStateAndIsMarkedAsVirtual()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public class MyActorState
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor, Virtual]
        public class MyActor : IMyActor
        {
          [State] private MyActorState _state;

          public MyActor(MyActorState state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR008_WhenActorHasInitializableSimpleStateAndIsMarkedAsVirtual()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor, Virtual]
        public class MyActor : IMyActor
        {
          [State] private int _state;

          public MyActor(int state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR008_WhenActorHasDefaultStateValueForStateAndIsMarkedAsVirtual()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor, Virtual]
        public class MyActor : IMyActor
        {
          [State(Default = "default")] private string _state;

          public MyActor(string state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldTriggerCAEPACTR009_WhenDuplicateActorImplementationAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public abstract class MyActor : IMyActor
        {
        }

        [ActorImplementation(typeof(MyActor)), ActorImplementation<MyActor>]
        public class MyImplementation : MyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR009, 14, 4, "ActorImplementation(typeof(MyActor))"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR010_WhenMultipleDefaultImplementations()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public abstract class MyActor : IMyActor
        {
        }
      
        [ActorImplementation<MyActor>(IsDefault = true)]
        public class MyImplementation1 : MyActor
        {
        }
      
        [ActorImplementation<MyActor>(IsDefault = true)]
        public class MyImplementation2 : MyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR010, 10, 25, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR011_WhenImplementationDoesNotInheritActor()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        [ActorImplementation<MyImplementation>]
        public class MyImplementation
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR011, 6, 16, "MyImplementation"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR012_WhenImplementationIsAbstract()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public abstract class MyActor : IMyActor
        {
        }
      
        [ActorImplementation<MyActor>]
        public abstract class MyImplementation : MyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR012, 15, 25, "MyImplementation"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR300_WhenStateTypeIsInterface()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IOther
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private IOther _other;
      
          public MyActor(IOther other)
          {
            _other = other;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR300, 16, 6, "State"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR300_WhenStateTypeIsAbstractClass()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public abstract class Other
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private Other _other;

          public MyActor(Other other)
          {
            _other = other;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR300, 16, 6, "State"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR300_WhenStateTypeIsDictionaryWithInvalidKeyType()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Collections.Generic;

      namespace Actors.Tests
      {
        public interface IOther
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private IDictionary<IOther, int> _dictionary;
      
          public MyActor(IDictionary<IOther, int> dictionary)
          {
            _dictionary = dictionary;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR300, 17, 6, "State"));
  }

  [Theory]
  [InlineData("IEnumerable")]
  [InlineData("IReadOnlyCollection")]
  [InlineData("ICollection")]
  [InlineData("IReadOnlyList")]
  [InlineData("IList")]
  [InlineData("List")]
  [InlineData("LinkedList")]
  [InlineData("ISet")]
  [InlineData("HashSet")]
  [InlineData("SortedSet")]
  [InlineData("Queue")]
  public void ShouldNotTriggerCAEPACTR300_WhenStateTypeIsCollection(string collectionType)
  {
    // Arrange
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System.Collections.Generic;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private {{collectionType}}<int> _collection;
      
          public MyActor({{collectionType}}<int> collection)
          {
            _collection = collection;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Theory]
  [CAEPACTR300ValidDictionaryData]
  public void ShouldNotTriggerCAEPACTR300_WhenStateTypeIsDictionaryWithValidKeyType(string dictionaryType, string keyType)
  {
    // Arrange
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System.Collections.Generic;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private {{dictionaryType}}<{{keyType}}, int> _dictionary;
      
          public MyActor({{dictionaryType}}<{{keyType}}, int> dictionary)
          {
            _dictionary = dictionary;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldTriggerCAEPACTR301_WhenDefaultValueIsInvalid()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State(Default = 1)] private string _state;

          public MyActor(string state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR301, 12, 6, "State(Default = 1)"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR302_WhenImplementationDefinesState()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public abstract class MyActor : IMyActor
        {
        }
      
        [ActorImplementation<MyActor>]
        public class MyImplementation : MyActor
        {
          [State] private int _state;

          public MyImplementation(int state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR302, 17, 6, "State"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR303_WhenActorIdSourceIsAmbiguous()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State, ActorId] private int _state1;
          [State, ActorId] private int _state2;
      
          public MyActor(int state1, int state2)
          {
            _state1 = state1;
            _state2 = state2;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR303, 10, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR304_WhenIdTypeIsInvalid()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public class ActorId { }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State, ActorId] private ActorId _id;

          public MyActor(ActorId id)
          {
            _id = id;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR304, 14, 38, "_id"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR305_WhenIdTypeImplementsIActorIdSourceMultipleTimes()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public class ActorId : IActorIdSource<int>, IActorIdSource<System.Guid>
        {
          int IActorIdSource<int>.GetActorId() => throw new System.NotImplementedException();
          void IActorIdSource<int>.SetActorId(int actorId) => throw new System.NotImplementedException();

          System.Guid IActorIdSource<System.Guid>.GetActorId() => throw new System.NotImplementedException();
          void IActorIdSource<System.Guid>.SetActorId(System.Guid actorId) => throw new System.NotImplementedException();
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State, ActorId] private ActorId _id;

          public MyActor(ActorId id)
          {
            _id = id;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR305, 5, 16, "ActorId"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR400_WhenStateComponentsCannotBeMatchedByParameter()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private string _state;
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR400, 10, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR400_WhenStateFieldCannotBeMatchedByConvention()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private string STATE;

          public MyActor(string state)
          {
            STATE = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR400, 14, 12, "MyActor"));
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR400_WhenStateFieldIsMatchedByMPrefix()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private string m_state;

          public MyActor(string state)
          {
            m_state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR400_WhenStateFieldHasSameName()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private string state;

          public MyActor(string state)
          {
            this.state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR400_WhenStateFieldIsBackingField()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public MyActor(string state)
          {
            State = state;
          }

          [State] public string State { get; set; }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldTriggerCAEPACTR400_WhenMoreThanOneStateFieldIsMatchedByConvention()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private string _state;

          public MyActor(string state)
          {
            _state = state;
            State = state;
          }

          [State] public string State { get; set; }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR400, 14, 12, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR401_WhenActorHasMoreThanOneConstructorNeitherMarkedWithAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyService1
        {
        }

        public interface IMyService2
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public MyActor(IMyService1 service)
          {
          }

          public MyActor(IMyService2 service)
          {
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR401, 18, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR401_WhenActorHasMoreThanOneConstructorBothMarkedWithAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyService1
        {
        }

        public interface IMyService2
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [ActorConstructor]
          public MyActor(IMyService1 service)
          {
          }
      
          [ActorConstructor]
          public MyActor(IMyService2 service)
          {
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR401, 18, 16, "MyActor"));
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR401_WhenActorHasMoreThanOneConstructorButOnlyOneMarkedWithAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyService1
        {
        }

        public interface IMyService2
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [ActorConstructor]
          public MyActor(IMyService1 service)
          {
          }
      
          public MyActor(IMyService2 service)
          {
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldTriggerCAEPACTR402_WhenActorContextOfWrongType()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public class Other
        {
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public MyActor(IActorContext<Other> context)
          {
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR402, 16, 41, "context"));
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR402_WhenActorContextOfCorrectType()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public MyActor(IActorContext<MyActor> context)
          {
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldNotTriggerCAEPACTR402_WhenUntypedActorContext()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public MyActor(IActorContext context)
          {
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldTriggerCAEPACTR403_WhenInterfaceMethodIsGeneric()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
          T MyMethod<T>();
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public T MyMethod<T>() => throw new System.NotImplementedException();
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR403, 13, 14, "MyMethod"));
  }

  [Theory]
  [InlineData("int")]
  [InlineData("void")]
  public void ShouldTriggerCAEPACTR404_WhenMethodHasInvalidReturnType(string type)
  {
    // Arrange
    string source = $$"""
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
          {{type}} MyMethod();
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public {{type}} MyMethod() => throw new System.NotImplementedException();
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR404, 13, 13 + type.Length, "MyMethod"));
  }

  [Theory]
  [InlineData("Task")]
  [InlineData("ValueTask")]
  [InlineData("Task<int>")]
  [InlineData("ValueTask<int>")]
  public void ShouldNotTriggerCAEPACTR100_WhenMethodHasValidReturnType(string returnType)
  {
    // Arrange
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System.Threading.Tasks;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
          {{returnType}} MyMethod();
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public {{returnType}} MyMethod() => throw new System.NotImplementedException();
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldTriggerCAEPACTR405_WhenCancellationTokenIsNotLastParameter()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public Task MyMethod(CancellationToken cancellationToken, int arg) => throw new System.NotImplementedException();
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR405, 14, 44, "cancellationToken"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR405_WhenDuplicateCancellationTokenParameters()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public Task MyMethod(CancellationToken cancellationToken1, int arg, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR405, 14, 44, "cancellationToken1"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR405_WhenDuplicateCancellationTokenParametersInMultipleLocations()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          public Task MyMethod(CancellationToken cancellationToken1, int arg, CancellationToken cancellationToken2, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(2)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR405, 14, 44, "cancellationToken1"))
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR405, 14, 91, "cancellationToken2"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR600_WhenInterfaceHasDuplicateActorFactoryAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests
      {
        [ActorFactory<MyActor>, ActorFactory(typeof(MyActor))]
        public interface IMyActorFactory
        {
          Task<IMyActor> CreateAsync(string id, int state, CancellationToken cancellationToken = default);
      
          IMyActor Get(string id);
        }
      
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private int _state;
      
          public MyActor(int state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR600, 7, 27, "ActorFactory(typeof(MyActor))"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR601_WhenInterfaceIsGeneric()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests
      {
        [ActorFactory<MyActor>]
        public interface IMyActorFactory<T>
        {
          Task<IMyActor> CreateAsync(string id, int state, CancellationToken cancellationToken = default);
      
          IMyActor Get(string id);
        }
      
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private int _state;
      
          public MyActor(int state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR601, 8, 20, "IMyActorFactory"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR602_WhenFactoryIsNotFound()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.CodeAnalysis;
      
      [assembly: DisableActorFactoryGeneration]

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR602, 13, 16, "MyActor"));
  }

  [Theory]
  [CAEPACTR603WrongSignatureData]
  public void ShouldTriggerCAEPACTR603_WhenInterfaceHasWrongSignature(string createAsyncReturnType, string createAsyncParameters, string getReturnType, string getParameters)
  {
    // Arrange
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests
      {
        [ActorFactory<MyActor>]
        public interface IMyActorFactory
        {
          {{createAsyncReturnType}} CreateAsync({{createAsyncParameters}});
      
          {{getReturnType}} Get({{getParameters}});
        }
      
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private int _state;
      
          public MyActor(int state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR603, 8, 20, "IMyActorFactory"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR604_WhenMultipleFactoriesTargetTheSameActor()
  {
    // Arrange
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests
      {
        [ActorFactory<MyActor>]
        public interface IMyActorFactory1
        {
          Task<IMyActor> CreateAsync(string id, int state, CancellationToken cancellationToken = default);
      
          IMyActor Get(string id);
        }

        [ActorFactory<MyActor>]
        public interface IMyActorFactory2
        {
          Task<IMyActor> CreateAsync(string id, int state, CancellationToken cancellationToken = default);
      
          IMyActor Get(string id);
        }
      
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private int _state;
      
          public MyActor(int state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR604, 16, 20, "IMyActorFactory2"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR700_WhenMessageIsNotActorMessage()
  {
    // Arrange
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Messaging;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }

        public class MyActorMessage { }
        
        [Actor]
        public class MyActor : IMyActor, IMessageHandler<MyActorMessage>
        {
          public Task HandleAsync(MyActorMessage message, CancellationToken cancellationToken)
            => throw new System.NotImplementedException();
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR700, 15, 16, "MyActor"));
  }

  [Fact]
  public void ShouldTriggerCAEPACTR700_WhenMessageIsActorMessageWithWrongIdType()
  {
    // Arrange
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.Messaging;
      using CodeArchitects.Platform.Messaging;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }

        public class MyActorMessage : IActorMessage<int>
        {
          public int ActorId { get; set; }
        }
        
        [Actor]
        public class MyActor : IMyActor, IMessageHandler<MyActorMessage>
        {
          public Task HandleAsync(MyActorMessage message, CancellationToken cancellationToken)
            => throw new System.NotImplementedException();
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().HaveCount(1)
      .And.ContainSingle(MatchDiagnostic(DiagnosticIds.CAEPACTR700, 19, 16, "MyActor"));
  }

  [Fact]
  public void ShouldNotTriggerFactorDiagnostics_WhenFactoryHasValidSignature()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.CodeAnalysis;
      using System.Threading;
      using System.Threading.Tasks;
      
      [assembly: DisableActorFactoryGeneration]

      namespace Actors.Tests
      {
        [ActorFactory<MyActor>]
        public interface IMyActorFactory
        {
          Task<IMyActor> CreateAsync(string id, int state, CancellationToken cancellationToken = default);

          IMyActor Get(string id);
        }

        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
          [State] private int _state;

          public MyActor(int state)
          {
            _state = state;
          }
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }

  [Fact]
  public void ShouldNotGenerateFactory_WhenAssemblyHasDisableActorFactoryGenerationAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.CodeAnalysis;
      
      [assembly: DisableActorFactoryGeneration]

      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor]
        public class MyActor : IMyActor
        {
        }

        [ActorFactory<MyActor>]
        public interface IMyCustomActorFactory
        {
          IMyActor Get(string id);
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
    compilation.GetTypeByMetadataName("Actors.Tests.IMyActorFactory").Should().BeNull();
  }

  [Fact]
  public void ShouldNotTriggerAnyDiagnostics_WhenAssemblyHasDisableActorDiagnosticAttribute()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.CodeAnalysis;
      
      [assembly: DisableActorDiagnostics]
      
      namespace Actors.Tests
      {
        public interface IMyActor
        {
        }
        
        [Actor, Actor<IMyActor>]
        public class MyActor : IMyActor
        {
        }
      }
      """;

    // Act
    CompileWithGenerator(source, out _, out ImmutableArray<Diagnostic> diagnostics);

    // Assert
    diagnostics.Should().BeEmpty();
  }
}
