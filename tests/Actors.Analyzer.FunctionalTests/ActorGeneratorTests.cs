using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer;

public partial class ActorGeneratorTests
{
  [Fact]
  public void ShouldGenerateValidFactory_WhenActorHasImplicitDefaultConstructor()
  {
    // Arrange
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
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    AssertValidFactoryType(compilation, true, false, null);
  }

  [Fact]
  public void ShouldGenerateValidFactory_WhenActorHasParameterlessConstructor()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;

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
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    AssertValidFactoryType(compilation, true, false, null);
  }

  [Fact]
  public void ShouldGenerateValidFactory_WhenActorIsStateless()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;

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
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    AssertValidFactoryType(compilation, true, false, null);
  }

  [Fact]
  public void ShouldGenerateValidFactory_WhenActorHasOneStateComponent()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;

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
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    AssertValidFactoryType(compilation, false, false, null,
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int32), "state0"));
  }

  [Fact]
  public void ShouldGenerateValidFactory_WhenActorHasMultipleStateComponents()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;

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
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    AssertValidFactoryType(compilation, false, false, null,
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int32), "state0"),
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int16), "state1"));
  }

  [Fact]
  public void ShouldGenerateValidFactory_WhenActorInterfaceIsGeneric()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;

      public interface IMyService { }

      public interface IMyActor<T>
      {
      }

      [Actor]
      public class MyActor : IMyActor<int>
      {
          [State] private int _state0;
          [State] private short _state1;
          
          public MyActor(int state0, IMyService service, IActorContext context, short state1)
          {
              _state0 = state0;
              _state1 = state1;
          }
      }
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    AssertValidFactoryType(compilation, false, false, null,
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int32), "state0"),
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int16), "state1"));
  }

  [Fact]
  public void ShouldGenerateValidFactory_WhenActorIsVirtual()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;
      
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
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    AssertValidFactoryType(compilation, true, false, null,
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int32), "state0"),
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int16), "state1"));
  }

  [Fact]
  public void ShouldGenerateValidFactory_WhenActorHasCustomIdType()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;
      using System;

      namespace Actors.Tests;

      public interface IMyActor
      {
      }
      
      [Actor, ActorIdType<Guid>]
      public class MyActor : IMyActor
      {
      }
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    AssertValidFactoryType(compilation, true, false, "System.Guid");
  }

  [Fact]
  public void ShouldGenerateValidFactory_WhenStateComponentIsActorId()
  {
    // Arrange
    string source = """
      using CodeArchitects.Platform.Actors;

      namespace Actors.Tests;

      public interface IMyActor
      {
      }
      
      [Actor]
      public class MyActor : IMyActor
      {
          [State, ActorId]
          private readonly int _state;
          
          public MyActor(int state)
          {
              _state = state;
          }
      }
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    AssertValidFactoryType(compilation, false, true, "System.Int32",
      new StateSpec(compilation.GetSpecialType(SpecialType.System_Int32), "state"));
  }

  [Fact]
  public void ShouldNotGenerateFactory_WhenAssemblyHasDisableActorFactoryGenerationAttribute()
  {
    // Arrange
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

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    compilation.GetTypeByMetadataName("Actors.Tests.IMyActorFactory").Should().BeNull();
  }

  [Fact]
  public void ShouldNotGenerateFactory_WhenFactoryAlreadyExists()
  {
    // Arrange
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
      
      [ActorFactory<MyActor>]
      public interface IMyCustomActorFactory
      {
          IMyActor Get(string id);
      }
      """;

    // Act
    Compilation compilation = CompileWithGenerator(source);

    // Assert
    compilation.GetTypeByMetadataName("Actors.Tests.IMyActorFactory").Should().BeNull();
  }
}
