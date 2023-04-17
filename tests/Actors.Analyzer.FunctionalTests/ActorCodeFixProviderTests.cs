using Verifier = CodeArchitects.Platform.Actors.Analyzer.Fixtures.ActorCodeFixVerifier;

namespace CodeArchitects.Platform.Actors.Analyzer;

public partial class ActorCodeFixProviderTests
{
  [Fact]
  public async Task ShouldFixCAEPACTR000_WhenSingleTypeParameter()
  {
    string source = """
       using CodeArchitects.Platform.Actors;
   
       namespace Test;
       
       [Actor]
       public class MyActor<T> : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;
       
       namespace Test;
       
       [Actor]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR000)
      .WithArguments("MyActor")
      .WithSpan(6, 14, 6, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR000_WhenMultipleTypeParameter()
  {
    string source = """
       using CodeArchitects.Platform.Actors;
   
       namespace Test;
       
       [Actor]
       public class MyActor<T1, T2> : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;
       
       namespace Test;
       
       [Actor]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR000)
      .WithArguments("MyActor")
      .WithSpan(6, 14, 6, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR000_WhenTypeParametersWithConstraints()
  {
    string source = """
       using CodeArchitects.Platform.Actors;
   
       namespace Test;
       
       [Actor]
       public class MyActor<T> : IMyActor
           where T : class
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;
       
       namespace Test;
       
       [Actor]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    var diagnostic = Verifier.Diagnostic(DiagnosticDescriptors.CAEPACTR000).WithArguments("MyActor").WithSpan(6, 14, 6, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR001_WhenTypeIsDefinedInFileScopedNamespace()
  {
    string source = """
       using CodeArchitects.Platform.Actors;

       namespace Test;

       [Actor]
       public class MyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;

       namespace Test;

       [Actor]
       public class MyActor : IMyActor
       {
       }
       """;

    string interfaceNewSource = """
      namespace Test;

      public interface IMyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR001)
      .WithArguments("MyActor")
      .WithSpan(6, 14, 6, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource)
      .AddFixedSource("IMyActor.cs", interfaceNewSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR001_WhenTypeIsDefinedInBlockNamespace()
  {
    string source = """
       using CodeArchitects.Platform.Actors;

       namespace Test
       {
           [Actor]
           public class MyActor
           {
           }
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;

       namespace Test
       {
           [Actor]
           public class MyActor : IMyActor
           {
           }
       }
       """;

    string interfaceNewSource = """
      namespace Test
      {
          public interface IMyActor
          {
          }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR001)
      .WithArguments("MyActor")
      .WithSpan(6, 18, 6, 25);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource)
      .AddFixedSource("IMyActor.cs", interfaceNewSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR001_WhenTypeIsNotDefinedInNamespace()
  {
    string source = """
       using CodeArchitects.Platform.Actors;

       [Actor]
       public class MyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;

       [Actor]
       public class MyActor : IMyActor
       {
       }

       public interface IMyActor
       {
       }
       """;

    var diagnostic = Verifier.Diagnostic(DiagnosticDescriptors.CAEPACTR001).WithArguments("MyActor").WithSpan(4, 14, 4, 21);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR003_WhenInterfaceNameIsQualified()
  {
    string source = """
       using CodeArchitects.Platform.Actors;

       namespace Test
       {
           [Actor<Test.Interfaces.IMyActor>]
           public class MyActor
           {
           }
       }

       namespace Test.Interfaces
       {
           public interface IMyActor
           {
           }
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;
       
       namespace Test
       {
           [Actor<Test.Interfaces.IMyActor>]
           public class MyActor : Test.Interfaces.IMyActor
           {
           }
       }
       
       namespace Test.Interfaces
       {
           public interface IMyActor
           {
           }
       }
       """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR003)
      .WithArguments("MyActor")
      .WithSpan(5, 12, 5, 36);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR003_WhenInterfaceNameIsSimple()
  {
    string source = """
       using CodeArchitects.Platform.Actors;

       namespace Test;
       
       [Actor<IMyActor>]
       public class MyActor
       {
       }

       public interface IMyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;
       
       namespace Test;
       
       [Actor<IMyActor>]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR003)
      .WithArguments("MyActor")
      .WithSpan(5, 8, 5, 16);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR003_WhenActorAttributeIsNonGeneric()
  {
    string source = """
       using CodeArchitects.Platform.Actors;

       namespace Test;
       
       [Actor(InterfaceType = typeof(IMyActor))]
       public class MyActor
       {
       }

       public interface IMyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;
       
       namespace Test;
       
       [Actor(InterfaceType = typeof(IMyActor))]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR003)
      .WithArguments("MyActor")
      .WithSpan(5, 31, 5, 39);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR008()
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
      
      [ActorImplementation<MyActor>(IsDefault = true)]
      public class MyImplementation3 : MyActor
      {
      }
      """;

    string fixedSource = """
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
      
      [ActorImplementation<MyActor>]
      public class MyImplementation2 : MyActor
      {
      }
      
      [ActorImplementation<MyActor>]
      public class MyImplementation3 : MyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR008)
      .WithArguments("MyActor")
      .WithSpan(10, 23, 10, 30);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR009_WhenImplementationAttributeIsGeneric()
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
      public class MyImplementation
      {
      }
      """;

    string fixedSource = """
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
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR009)
      .WithArguments("MyImplementation")
      .WithSpan(14, 22, 14, 29);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR009_WhenImplementationAttributeIsNonGeneric()
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
      
      [ActorImplementation(typeof(MyActor))]
      public class MyImplementation
      {
      }
      """;

    string fixedSource = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public abstract class MyActor : IMyActor
      {
      }
      
      [ActorImplementation(typeof(MyActor))]
      public class MyImplementation : MyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR009)
      .WithArguments("MyImplementation")
      .WithSpan(14, 29, 14, 36);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR010()
  {
    // 'partial' added so that 'abstract' is placed between two modifiers
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public abstract partial class MyActor : IMyActor
      {
      }
      """;

    string fixedSource = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor]
      public partial class MyActor : IMyActor
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR010)
      .WithArguments("MyActor")
      .WithSpan(10, 31, 10, 38);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR305_WhenActorIdTypeIsExplicit()
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
          [ActorId] private long _id;
      }
      """;

    string fixedSource = """
      using CodeArchitects.Platform.Actors;
      
      namespace Actors.Tests;
      
      public interface IMyActor
      {
      }
      
      [Actor, ActorIdType<int>]
      public class MyActor : IMyActor
      {
          [ActorId] private int _id;
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR305)
      .WithArguments("long", "int")
      .WithSpan(12, 28, 12, 31);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR305_WhenActorIdTypeIsImplicit()
  {
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests
      {
          using Actors.Tests.Ids;
          
          public interface IMyActor
          {
          }
          
          [Actor]
          public abstract class MyActor : IMyActor
          {
              [ActorId] private MyActorId _id;
          }
      }

      namespace Actors.Tests.Implementations
      {
          [ActorImplementation<MyActor>]
          public class MyImplementation : MyActor
          {
              [ActorId] private long _id;
          }
      }

      namespace Actors.Tests.Ids
      {
          using System;
          
          public class MyActorId
          {
              public static MyActorId Parse(string text) => throw new NotImplementedException();
          }
      }
      """;

    string fixedSource = """
      using CodeArchitects.Platform.Actors;
      
      namespace Actors.Tests
      {
          using Actors.Tests.Ids;
          
          public interface IMyActor
          {
          }
          
          [Actor]
          public abstract class MyActor : IMyActor
          {
              [ActorId] private MyActorId _id;
          }
      }
      
      namespace Actors.Tests.Implementations
      {
          [ActorImplementation<MyActor>]
          public class MyImplementation : MyActor
          {
              [ActorId] private Actors.Tests.Ids.MyActorId _id;
          }
      }
      
      namespace Actors.Tests.Ids
      {
          using System;
          
          public class MyActorId
          {
              public static MyActorId Parse(string text) => throw new NotImplementedException();
          }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR305)
      .WithArguments("long", "MyActorId")
      .WithSpan(23, 32, 23, 35);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR402()
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

    string fixedSource = """
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
          public MyActor(IActorContext<MyActor> context)
          {
          }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR402)
      .WithArguments("context", "MyActor")
      .WithSpan(16, 41, 16, 48);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Theory]
  [InlineData("int", "Task<int>")]
  [InlineData("void", "Task")]
  [InlineData("MyResult", "Task<MyResult>")]
  public async Task ShouldFixCAEPACTR404(string type, string fixedType)
  {
    string classSource = $$"""
      using CodeArchitects.Platform.Actors;
      using System;

      namespace Actors.Tests;

      [Actor]
      public class MyActor : IMyActor
      {
          public {{type}} MyMethod() => throw new NotImplementedException();
      }
      """;

    string interfaceSource = $$"""
      using System.Threading.Tasks;
      
      namespace Actors.Tests;
      
      public interface IMyActor
      {
          {{type}} MyMethod();
      }
      """;

    string resultSource = $$"""
      namespace Actors.Tests;

      public class MyResult { }
      """;

    string fixedClassSource = $$"""
      using CodeArchitects.Platform.Actors;
      using System;
      using System.Threading.Tasks;

      namespace Actors.Tests;

      [Actor]
      public class MyActor : IMyActor
      {
          public {{fixedType}} MyMethodAsync() => throw new NotImplementedException();
      }
      """;

    string fixedInterfaceSource = $$"""
      using System.Threading.Tasks;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
          {{fixedType}} MyMethodAsync();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR404)
      .WithArguments("MyMethod", type)
      .WithSpan(9, 13 + type.Length, 9, 21 + type.Length);

    await Verifier.Verify(verifier => verifier
      .AddSource(classSource, diagnostic)
      .AddSource(interfaceSource)
      .AddSource(resultSource)
      .AddFixedSource(fixedClassSource)
      .AddFixedSource(fixedInterfaceSource)
      .AddFixedSource(resultSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR405_WhenCancellationTokenIsNotLastParameter()
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

    string fixedSource = """
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
          public Task MyMethod(int arg, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR405)
      .WithArguments("MyMethod")
      .WithSpan(15, 44, 15, 61);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR405_WhenDuplicateCancellationTokenParametersInMultipleLocations()
  {
    string source = """
      using CodeArchitects.Platform.Actors;
      using System;
      using System.Threading;
      using System.Threading.Tasks;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
          Task MyMethod(CancellationToken cancellationToken1, int arg, CancellationToken cancellationToken2, CancellationToken cancellationToken);
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public Task MyMethod(CancellationToken cancellationToken1, int arg, CancellationToken cancellationToken2, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      """;

    string fixedSource = """
      using CodeArchitects.Platform.Actors;
      using System;
      using System.Threading;
      using System.Threading.Tasks;

      namespace Actors.Tests;
      
      public interface IMyActor
      {
          Task MyMethod(int arg, CancellationToken cancellationToken1);
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          public Task MyMethod(int arg, CancellationToken cancellationToken1) => throw new NotImplementedException();
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR405)
      .WithArguments("MyMethod");

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic.WithSpan(16, 44, 16, 62), diagnostic.WithSpan(16, 91, 16, 109))
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR700_WhenExtraAttributeIsBottom()
  {
    string source = """
       using CodeArchitects.Platform.Actors;
   
       namespace Test;
       
       [Actor<IMyActor>]
       [Actor]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;
       
       namespace Test;
       
       [Actor<IMyActor>]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR700)
      .WithArguments("Actor")
      .WithSpan(6, 2, 6, 7);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR700_WhenExtraAttributeIsTop()
  {
    string source = """
       using CodeArchitects.Platform.Actors;
   
       namespace Test;
       
       [Actor]
       [Actor<IMyActor>]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;
       
       namespace Test;
       
       [Actor<IMyActor>]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR700)
      .WithArguments("Actor")
      .WithSpan(5, 2, 5, 7);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR700_WhenExtraAttributeIsInSameList()
  {
    string source = """
       using CodeArchitects.Platform.Actors;
   
       namespace Test;
       
       [Actor, Actor<IMyActor>]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    string fixedSource = """
       using CodeArchitects.Platform.Actors;
       
       namespace Test;
       
       [Actor<IMyActor>]
       public class MyActor : IMyActor
       {
       }
       
       public interface IMyActor
       {
       }
       """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR700)
      .WithArguments("Actor")
      .WithSpan(5, 2, 5, 7);

    await Verifier.Verify(verifier => verifier
      .AddSource(source, diagnostic)
      .AddFixedSource(fixedSource));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR702_WhenMessageImplementsWrongInterface()
  {
    string actorSource = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Messaging;
      using System;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;

      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor, IMessageHandler<MyActorMessage>
      {
          public Task HandleAsync(MyActorMessage message, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      """;

    string messageSource = """
      using CodeArchitects.Platform.Actors.Messaging;
      
      namespace Actors.Tests;

      public class MyActorMessage : IActorMessage<int>
      {
          public int ActorId { get; set; }
      }
      """;

    string fixedMessageSource = """
      using CodeArchitects.Platform.Actors.Messaging;

      namespace Actors.Tests;
      
      public class MyActorMessage : IActorMessage<string>
      {
          public int ActorId { get; set; }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR702)
      .WithArguments("MyActorMessage")
      .WithSpan(14, 34, 14, 65);

    var afterFixDiagnostic = Verifier
      .Diagnostic("CS0738")
      .WithSpan(5, 31, 5, 52);

    await Verifier.Verify(verifier => verifier
      .AddSource(actorSource, diagnostic)
      .AddSource(messageSource)
      .AddFixedSource(actorSource)
      .AddFixedSource(fixedMessageSource, afterFixDiagnostic));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR702_WhenMessageDoesNotImplementsAnyInterface()
  {
    string actorSource = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Messaging;
      using System;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;

      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor, IMessageHandler<MyActorMessage>
      {
          public Task HandleAsync(MyActorMessage message, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      """;

    string messageSource = """
      namespace Actors.Tests;
      
      public class MyActorMessage
      {
      }
      """;

    string fixedMessageSource = """
      using CodeArchitects.Platform.Actors.Messaging;
      namespace Actors.Tests;
      
      public class MyActorMessage : IActorMessage<string>
      {
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR702)
      .WithArguments("MyActorMessage")
      .WithSpan(14, 34, 14, 65);

    var afterFixDiagnostic = Verifier
      .Diagnostic("CS0535")
      .WithSpan(4, 31, 4, 52);

    await Verifier.Verify(verifier => verifier
      .AddSource(actorSource, diagnostic)
      .AddSource(messageSource)
      .AddFixedSource(actorSource)
      .AddFixedSource(fixedMessageSource, afterFixDiagnostic));
  }

  [Fact]
  public async Task ShouldFixCAEPACTR702_WhenMessageImplementsInterfacesButNotIActorMessage()
  {
    string actorSource = """
      using CodeArchitects.Platform.Actors;
      using CodeArchitects.Platform.Messaging;
      using System;
      using System.Threading;
      using System.Threading.Tasks;
      
      namespace Actors.Tests;

      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor, IMessageHandler<MyActorMessage>
      {
          public Task HandleAsync(MyActorMessage message, CancellationToken cancellationToken) => throw new NotImplementedException();
      }
      """;

    string messageSource = """
      using System;

      namespace Actors.Tests;
      
      public class MyActorMessage : IDisposable
      {
          void IDisposable.Dispose() { }
      }
      """;

    string fixedMessageSource = """
      using System;
      using CodeArchitects.Platform.Actors.Messaging;

      namespace Actors.Tests;
      
      public class MyActorMessage : IDisposable, IActorMessage<string>
      {
          void IDisposable.Dispose() { }
      }
      """;

    var diagnostic = Verifier
      .Diagnostic(DiagnosticDescriptors.CAEPACTR702)
      .WithArguments("MyActorMessage")
      .WithSpan(14, 34, 14, 65);

    var afterFixDiagnostic = Verifier
      .Diagnostic("CS0535")
      .WithSpan(6, 44, 6, 65);

    await Verifier.Verify(verifier => verifier
      .AddSource(actorSource, diagnostic)
      .AddSource(messageSource)
      .AddFixedSource(actorSource)
      .AddFixedSource(fixedMessageSource, afterFixDiagnostic));
  }
}
