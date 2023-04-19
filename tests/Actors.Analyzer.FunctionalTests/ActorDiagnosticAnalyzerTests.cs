using Verifier = CodeArchitects.Platform.Actors.Analyzer.Fixtures.ActorAnalyzerVerifier;

namespace CodeArchitects.Platform.Actors.Analyzer;

public partial class ActorDiagnosticAnalyzerTests
{
  [Fact]
  public async Task ShouldTriggerCAEPACTR000_WhenClassIsGeneric()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;

      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor<T> : IMyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR000)
      .WithArguments("MyActor")
      .WithSpan(10, 14, 10, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR000Only_WhenClassIsGenericAndThereAreImplementations()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;

      public interface IMyActor
      {
      }
      
      [Actor]
      public abstract class MyActor<T> : IMyActor
      {
      }

      [ActorImplementation<MyActor>]
      public class MyImplementation : MyActor
      {
      }
      """;

    var diagnostics = new[]
    {
      Verifier
        .Diagnostic(DiagnosticDescriptors.CAEPACTR000)
        .WithArguments("MyActor")
        .WithSpan(10, 23, 10, 30),
      Verifier
        .Diagnostic("CS0305")
        .WithSpan(14, 22, 14, 29)
        .WithArguments("Actors.Tests.MyActor<T>", "type", "1"),
      Verifier
        .Diagnostic("CS0305")
        .WithSpan(15, 33, 15, 40)
        .WithArguments("Actors.Tests.MyActor<T>", "type", "1")
    };

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostics));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR001_WhenClassDoesNotImplementAnyInterface()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR001)
      .WithArguments("MyActor")
      .WithSpan(10, 14, 10, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR001_WhenClassImplementsWellKnownInterfacesOnly()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.Messaging;
      using CodeArchitects.Platform.Messaging;
      using System;
      using System.Threading;
      using System.Threading.Tasks;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMessageHandler<ActorMessage>
      {
          public Task HandleAsync(ActorMessage message, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      
      public class ActorMessage : IActorMessage<string>
      {
          public string ActorId => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR001)
      .WithArguments("MyActor")
      .WithSpan(15, 14, 15, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR002_WhenClassImplementsMoreThanOneInterfaceAndTheActorInterfaceIsNotSpecified()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR002)
      .WithArguments("MyActor")
      .WithSpan(14, 14, 14, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR002_WhenClassImplementsMoreThanOneInterfaceAndTheActorInterfaceIsSpecifiedWithNonGenericAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR002_WhenClassImplementsMoreThanOneInterfaceAndTheActorInterfaceIsSpecifiedWithGenericAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR003_WhenClassDoesNotImplementTheInterfaceSpecifiedWithNonGenericAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR003)
      .WithArguments("MyActor")
      .WithSpan(13, 31, 13, 39);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR003_WhenClassDoesNotImplementTheInterfaceSpecifiedWithGenericAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR003)
      .WithArguments("MyActor")
      .WithSpan(13, 8, 13, 16);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR004_WhenInterfaceTypeSpecifiedWithNonGenericAttributeIsNotAnInterface()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR004)
      .WithArguments("Other")
      .WithSpan(13, 2, 13, 38);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR004_WhenInterfaceTypeSpecifiedWithGenericAttributeIsNotAnInterface()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR004)
      .WithArguments("Other")
      .WithSpan(13, 2, 13, 14);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR005_WhenInterfaceHasProperties()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
          int Property { get; }
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public int Property { get; }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR005)
      .WithArguments("IMyActor")
      .WithSpan(11, 14, 11, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR006_WhenInterfaceHasEvents()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
          event Action Event;
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public event Action Event;
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR006)
      .WithArguments("IMyActor")
      .WithSpan(12, 14, 12, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR007_WhenActorHasUninitializableStateAndIsMarkedAsVirtual()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR007)
      .WithArguments("MyActor")
      .WithSpan(14, 9, 14, 16);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR007_WhenActorHasInitializableComplexStateAndIsMarkedAsVirtual()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR007_WhenActorHasInitializableSimpleStateAndIsMarkedAsVirtual()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR007_WhenActorHasDefaultStateValueForStateAndIsMarkedAsVirtual()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR008_WhenMultipleDefaultImplementations()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR008)
      .WithArguments("MyActor")
      .WithSpan(10, 23, 10, 30);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR009_WhenImplementationDoesNotInheritActor()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }

      [Actor]
      public class MyActor : IMyActor
      {
      }
      
      [ActorImplementation<MyActor>]
      public class MyImplementation
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR009)
      .WithArguments("MyImplementation")
      .WithSpan(14, 22, 14, 29);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR010_WhenImplementationIsAbstract()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR010)
      .WithArguments("MyImplementation")
      .WithSpan(15, 23, 15, 39);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR010_WhenActorIsAbstractAndHasNoImplementations()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public abstract class MyActor : IMyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR010)
      .WithArguments("MyActor")
      .WithSpan(10, 23, 10, 30);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR300_WhenStateTypeIsInterface()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR300)
      .WithArguments("IOther")
      .WithSpan(16, 6, 16, 11);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR300_WhenStateTypeIsAbstractClass()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR300)
      .WithArguments("Other")
      .WithSpan(16, 6, 16, 11);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR300_WhenStateTypeIsDictionaryWithInvalidKeyType()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Collections.Generic;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR300)
      .WithArguments("IDictionary<IOther, int>")
      .WithSpan(17, 6, 17, 11);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
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
  public async Task ShouldNotTriggerCAEPACTR300_WhenStateTypeIsCollection(string collectionType)
  {
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System.Collections.Generic;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Theory]
  [CAEPACTR300ValidDictionaryData]
  public async Task ShouldNotTriggerCAEPACTR300_WhenStateTypeIsDictionaryWithValidKeyType(string dictionaryType, string keyType)
  {
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System.Collections.Generic;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR301_WhenDefaultValueIsInvalid()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR301)
      .WithArguments("_state")
      .WithSpan(12, 6, 12, 24);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR302_WhenImplementationDefinesState()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR302)
      .WithArguments("MyImplementation")
      .WithSpan(17, 6, 17, 11);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR303_WhenActorIdSourceIsAmbiguous()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR303)
      .WithSpan(10, 14, 10, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR304_WhenMemberIdTypeIsInvalid()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public class ActorId { }
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          [ActorId] private ActorId _id;
          
          public MyActor(ActorId id)
          {
              _id = id;
          }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR304)
      .WithArguments("ActorId")
      .WithSpan(14, 31, 14, 34);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR304_WhenActorIdTypeWithNonGenericAttributeIsInvalid()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public class ActorId { }
      
      public interface IMyActor
      {
      }
      
      [Actor, ActorIdType(typeof(ActorId))]
      public class MyActor : IMyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR304)
      .WithArguments("ActorId")
      .WithSpan(11, 9, 11, 37);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR304_WhenActorIdTypeWithGenericAttributeIsInvalid()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public class ActorId { }
      
      public interface IMyActor
      {
      }
      
      [Actor, ActorIdType<ActorId>]
      public class MyActor : IMyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR304)
      .WithArguments("ActorId")
      .WithSpan(11, 9, 11, 29);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR305_WhenStateComponentActorIdIsOfWrongType()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor, ActorIdType<int>]
      public class MyActor : IMyActor
      {
          [State, ActorId] private string _state;
          
          public MyActor(string state)
          {
              _state = state;
          }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR305)
      .WithArguments("string", "int")
      .WithSpan(12, 37, 12, 43);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR305_WhenActorIdMemberIsOfWrongType()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor, ActorIdType<int>]
      public class MyActor : IMyActor
      {
          [ActorId] private string _id;
          
          public MyActor(string id)
          {
              _id = id;
          }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR305)
      .WithArguments("string", "int")
      .WithSpan(12, 30, 12, 33);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR305_WhenImplementationDefinesIdOfWrongType()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor, ActorIdType<int>]
      public abstract class MyActor : IMyActor
      {
      }
      
      [ActorImplementation<MyActor>]
      public class MyImplementation : MyActor
      {
          [ActorId] private string _id;
          
          public MyImplementation(string id)
          {
              _id = id;
          }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR305)
      .WithArguments("string", "int")
      .WithSpan(17, 30, 17, 33);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR306_WhenStateComponentActorIdIsOfCorrectType()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor, ActorIdType<int>]
      public class MyActor : IMyActor
      {
          [State, ActorId] private int _state;
          
          public MyActor(int state)
          {
              _state = state;
          }
      }
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR400_WhenStateComponentsCannotBeMatchedByParameter()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          [State] private string _state;
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR400)
      .WithSpan(10, 14, 10, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR400_WhenStateFieldCannotBeMatchedByConvention()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR400)
      .WithSpan(14, 12, 14, 19);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR400_WhenStateFieldIsMatchedByMPrefix()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR400_WhenStateFieldHasSameName()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR400_WhenStateFieldIsBackingField()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR400_WhenMoreThanOneStateFieldIsMatchedByConvention()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR400)
      .WithSpan(14, 12, 14, 19);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR401_WhenActorHasMoreThanOneConstructorNeitherMarkedWithAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR401)
      .WithSpan(18, 14, 18, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR401_WhenActorHasMoreThanOneConstructorBothMarkedWithAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR401)
      .WithSpan(18, 14, 18, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR401_WhenActorHasMoreThanOneConstructorButOnlyOneMarkedWithAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR402_WhenActorContextOfWrongType()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR402)
      .WithArguments("context", "MyActor")
      .WithSpan(16, 41, 16, 48);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR402_WhenActorContextOfCorrectType()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldNotTriggerCAEPACTR402_WhenUntypedActorContext()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR403_WhenInterfaceMethodIsGeneric()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
          T MyMethod<T>();
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public T MyMethod<T>() => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR403)
      .WithArguments("MyMethod")
      .WithSpan(14, 14, 14, 22);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Theory]
  [InlineData("int")]
  [InlineData("void")]
  public async Task ShouldTriggerCAEPACTR404_WhenMethodHasInvalidReturnType(string type)
  {
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
          {{type}} MyMethod();
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public {{type}} MyMethod() => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR404)
      .WithArguments("MyMethod", type)
      .WithSpan(14, 13 + type.Length, 14, 21 + type.Length);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Theory]
  [InlineData("Task")]
  [InlineData("ValueTask")]
  [InlineData("Task<int>")]
  [InlineData("ValueTask<int>")]
  public async Task ShouldNotTriggerCAEPACTR404_WhenMethodHasValidReturnType(string returnType)
  {
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System;
      using System.Threading.Tasks;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
          {{returnType}} MyMethod();
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public {{returnType}} MyMethod() => throw new NotImplementedException();
      }
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR405_WhenCancellationTokenIsNotLastParameter()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System;
      using System.Threading;
      using System.Threading.Tasks;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public Task MyMethod(CancellationToken cancellationToken, int arg) => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR405)
      .WithArguments("MyMethod")
      .WithSpan(15, 44, 15, 61);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR405_WhenDuplicateCancellationTokenParameters()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System;
      using System.Threading;
      using System.Threading.Tasks;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public Task MyMethod(CancellationToken cancellationToken1, int arg, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR405)
      .WithArguments("MyMethod")
      .WithSpan(15, 44, 15, 62);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR405_WhenDuplicateCancellationTokenParametersInMultipleLocations()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System;
      using System.Threading;
      using System.Threading.Tasks;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public Task MyMethod(CancellationToken cancellationToken1, int arg, CancellationToken cancellationToken2, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR405)
      .WithArguments("MyMethod");

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic.WithSpan(15, 44, 15, 62), diagnostic.WithSpan(15, 91, 15, 109)));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR600_WhenFactoryIsGeneric()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR600)
      .WithArguments("IMyActorFactory")
      .WithSpan(8, 18, 8, 33);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR601_WhenFactoryIsNotFound()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.CodeAnalysis;
      
      [assembly: DisableActorFactoryGeneration]

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR601)
      .WithArguments("MyActor")
      .WithSpan(13, 14, 13, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Theory]
  [CAEPACTR602WrongSignatureData]
  public async Task ShouldTriggerCAEPACTR602_WhenFactoryHasWrongSignature(string createAsyncReturnType, string createAsyncParameters, string getReturnType, string getParameters)
  {
    string source = $$"""
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR602)
      .WithArguments("IMyActorFactory")
      .WithSpan(8, 18, 8, 33);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR603_WhenMultipleFactoriesTargetTheSameActor()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR603)
      .WithArguments("MyActor")
      .WithSpan(28, 14, 28, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR700_WhenActorHasDuplicateActorAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor, Actor<IMyActor>]
      public class MyActor : IMyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR700)
      .WithArguments("Actor")
      .WithSpan(9, 2, 9, 7);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR700_WhenActorHasDuplicateActorIdTypeAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor, ActorIdType(typeof(short)), ActorIdType<int>]
      public class MyActor : IMyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR700)
      .WithArguments("ActorIdType")
      .WithSpan(9, 9, 9, 35);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR700_WhenDuplicateActorImplementationAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR700)
      .WithArguments("ActorImplementation")
      .WithSpan(14, 2, 14, 38);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR700_WhenFactoryHasDuplicateActorFactoryAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;
      
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
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR700)
      .WithArguments("ActorFactory")
      .WithSpan(7, 25, 7, 54);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR701_WhenImplementationTargetIsNotAnActor()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      [ActorImplementation<MyImplementation>]
      public class MyImplementation
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR701)
      .WithArguments("MyImplementation")
      .WithSpan(6, 14, 6, 30);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR702_WhenMessageIsNotActorMessage()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Messaging;
      using System;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;

      public interface IMyActor
      {
      }
      
      public class MyActorMessage { }
      
      [Actor]
      public class MyActor : IMyActor, IMessageHandler<MyActorMessage>
      {
          public Task HandleAsync(MyActorMessage message, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR702)
      .WithArguments("MyActorMessage")
      .WithSpan(16, 34, 16, 65);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldTriggerCAEPACTR702_WhenMessageIsActorMessageWithWrongIdType()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.Messaging;
      using CodeArchitects.Platform.Messaging;
      using System;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;

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
          public Task HandleAsync(MyActorMessage message, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR702)
      .WithArguments("MyActorMessage")
      .WithSpan(20, 34, 20, 65);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic));
  }

  [Fact]
  public async Task ShouldNotTriggerDiagnostics_WhenFactoryHasValidSignature()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.CodeAnalysis;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;

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
      """;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }

  [Fact]
  public async Task ShouldNotTriggerAnyDiagnostics_WhenAssemblyHasDisableActorDiagnosticAttribute()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Actors.CodeAnalysis;
      
      [assembly: DisableActorDiagnostics]
      
      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor, Actor<IMyActor>]
      public class MyActor : IMyActor
      {
      }
      """
    ;

    await Verifier.Verify(verifier => verifier.AddSource(source));
  }
}
