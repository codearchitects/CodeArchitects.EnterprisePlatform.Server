using CodeArchitects.Platform.Common.Utils;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace CodeArchitects.Platform.Common.Tests.Utils
{
  public class CreationHelpersTests
  {
    private readonly Mock<IServiceProvider> _serviceProviderMock;

    public CreationHelpersTests()
    {
      _serviceProviderMock = new Mock<IServiceProvider>(behavior: MockBehavior.Strict);
      _serviceProviderMock
        .Setup(x => x.GetService(It.IsAny<Type>()))
        .Returns(null);
    }

    [Fact]
    public void CreateFromServices_ShouldCreateInstance_WhenTypeHasDefaultConstructor()
    {
      // Arrange

      // Act
      object result = CreationHelpers.CreateFromServices(_serviceProviderMock.Object, typeof(TypeWithDefaultCtor));

      // Assert
      result.Should().NotBeNull().And.BeOfType<TypeWithDefaultCtor>();
    }

    [Fact]
    public void CreateFromServices_ShouldThrowMissingMethodException_WhenTypeHasPrivateConstructor()
    {
      // Arrange

      // Act
      Func<object> act = () => CreationHelpers.CreateFromServices(_serviceProviderMock.Object, typeof(TypeWithPrivateCtor));

      // Assert
      act.Should().ThrowExactly<MissingMethodException>();
    }

    [Fact]
    public void CreateFromServices_ShouldCreateInstance_WhenTypeHasConstructorTakingSomeParameters()
    {
      // Arrange
      _serviceProviderMock
        .Setup(x => x.GetService(typeof(Service1)))
        .Returns(new Service1());
      _serviceProviderMock
        .Setup(x => x.GetService(typeof(Service2)))
        .Returns(new Service2());

      // Act
      object result = CreationHelpers.CreateFromServices(_serviceProviderMock.Object, typeof(TypeWithParametrizedCtor));

      // Assert
      result.Should().NotBeNull().And.BeOfType<TypeWithParametrizedCtor>();
    }

    [Fact]
    public void CreateFromServices_ShouldThrowInvalidOperationException_WhenDependenciesAreNotRegistered()
    {
      // Arrange
      _serviceProviderMock
        .Setup(x => x.GetService(typeof(Service1)))
        .Returns(new Service1());

      // Act
      Func<object> act = () => CreationHelpers.CreateFromServices(_serviceProviderMock.Object, typeof(TypeWithParametrizedCtor));

      // Assert
      act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void CreateFromServices_ShouldCreateInstance_AndUseBiggerConstructor_WhenTypeHasMoreThanOneConstructor()
    {
      // Arrange
      _serviceProviderMock
        .Setup(x => x.GetService(typeof(Service1)))
        .Returns(new Service1());
      _serviceProviderMock
        .Setup(x => x.GetService(typeof(Service2)))
        .Returns(new Service2());

      // Act
      object result = CreationHelpers.CreateFromServices(_serviceProviderMock.Object, typeof(TypeWith2Ctors));

      // Assert
      result.Should().NotBeNull().And.BeOfType<TypeWith2Ctors>().Which.ConstructorCalled.Should().Be(2);
    }

    [Fact]
    public void CreateFromServices_ShouldCreateInstance_AndFallBackToSmallerConstructor_WhenDependenciesOfBiggerConstructorsAreNotRegistered()
    {
      // Arrange
      _serviceProviderMock
        .Setup(x => x.GetService(typeof(Service1)))
        .Returns(new Service1());

      // Act
      object result = CreationHelpers.CreateFromServices(_serviceProviderMock.Object, typeof(TypeWith2Ctors));

      // Assert
      result.Should().NotBeNull().And.BeOfType<TypeWith2Ctors>().Which.ConstructorCalled.Should().Be(1);
    }

    private class TypeWithDefaultCtor { }

    private class TypeWithPrivateCtor
    {
      private TypeWithPrivateCtor() { }
    }

    private class TypeWithParametrizedCtor
    {
      public TypeWithParametrizedCtor(Service1 param1, Service2 param2) { }
    }

    private class TypeWith2Ctors
    {
      public TypeWith2Ctors(Service1 param) { ConstructorCalled = 1; }
      public TypeWith2Ctors(Service1 param1, Service2 param2) { ConstructorCalled = 2; }

      public int ConstructorCalled { get; }
    }

    private class Service1 { }

    private class Service2 { }
  }
}
