using FluentAssertions;
using System;
using System.Collections.Generic;

namespace CodeArchitects.Platform.Common.Utils;

public class TypeExtensionsTests
{
  [Fact]
  public void ImplementsGenericInterface_ShouldReturnFalse_WhenTypeDoesNotImplementInterface()
  {
    // Arrange

    // Act
    bool result = typeof(StubClass0).ImplementsGenericInterface(typeof(IGenericInterface<>));

    // Assert
    result.Should().BeFalse();
  }

  [Fact]
  public void ImplementsGenericInterface_ShouldReturnTrue_WhenTypeImplementsInterfaceExacltyOnce()
  {
    // Arrange

    // Act
    bool result = typeof(StubClass1).ImplementsGenericInterface(typeof(IGenericInterface<>));

    // Assert
    result.Should().BeTrue();
  }

  [Fact]
  public void ImplementsGenericInterface_ShouldReturnTrue_WhenTypeImplementsInterfaceMoreThanOnce()
  {
    // Arrange

    // Act
    bool result = typeof(StubClass2).ImplementsGenericInterface(typeof(IGenericInterface<>));

    // Assert
    result.Should().BeTrue();
  }

  [Fact]
  public void ImplementsGenericInterface_ShouldThrowArgumentException_WhenInterfaceTypeIsNotGeneric()
  {
    // Arrange

    // Act
    Func<bool> act = () => typeof(StubClass0).ImplementsGenericInterface(typeof(INonGenericInterface));

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }

  [Fact]
  public void ImplementsGenericInterface_ShouldThrowArgumentException_WhenInterfaceTypeIsNotInterface()
  {
    // Arrange

    // Act
    Func<bool> act = () => typeof(StubClass0).ImplementsGenericInterface(typeof(StubClass0));

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }

  [Fact]
  public void ImplementsGenericInterfaceExactlyOnce_ShouldReturnFalse_WhenTypeDoesNotImplementInterface()
  {
    // Arrange

    // Act
    bool result = typeof(StubClass0).ImplementsGenericInterfaceExactlyOnce(typeof(IGenericInterface<>));

    // Assert
    result.Should().BeFalse();
  }

  [Fact]
  public void ImplementsGenericInterfaceExacltyOnce_ShouldReturnTrue_WhenTypeImplementsInterfaceExacltyOnce()
  {
    // Arrange

    // Act
    bool result = typeof(StubClass1).ImplementsGenericInterfaceExactlyOnce(typeof(IGenericInterface<>));

    // Assert
    result.Should().BeTrue();
  }

  [Fact]
  public void ImplementsGenericInterfaceExactlyOnce_ShouldReturnFalse_WhenTypeImplementsInterfaceMoreThanOnce()
  {
    // Arrange

    // Act
    bool result = typeof(StubClass2).ImplementsGenericInterfaceExactlyOnce(typeof(IGenericInterface<>));

    // Assert
    result.Should().BeFalse();
  }

  [Fact]
  public void ImplementsGenericInterfaceExactlyOnce_ShouldThrowArgumentException_WhenInterfaceTypeIsNotGeneric()
  {
    // Arrange

    // Act
    Func<bool> act = () => typeof(StubClass0).ImplementsGenericInterfaceExactlyOnce(typeof(INonGenericInterface));

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }

  [Fact]
  public void ImplementsGenericInterfaceExactlyOnce_ShouldThrowArgumentException_WhenInterfaceTypeIsNotInterface()
  {
    // Arrange

    // Act
    Func<bool> act = () => typeof(StubClass0).ImplementsGenericInterfaceExactlyOnce(typeof(StubClass0));

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }

  [Fact]
  public void GetGenericInterfaces_ShouldReturnEmptyEnumerable_WhenTypeDoesNotImplementInterface()
  {
    // Arrange

    // Act
    IEnumerable<Type> types = typeof(StubClass0).GetGenericInterfaces(typeof(IGenericInterface<>));

    // Assert
    types.Should().BeEmpty();
  }

  [Fact]
  public void GetGenericInterfaces_ShouldReturnImplementedInterfaces_WhenTypeImplementsInterface()
  {
    // Arrange

    // Act
    IEnumerable<Type> types = typeof(StubClass2).GetGenericInterfaces(typeof(IGenericInterface<>));

    // Assert
    types.Should().HaveCount(2).And.Contain(new Type[] { typeof(IGenericInterface<int>), typeof(IGenericInterface<string>) });
  }

  [Fact]
  public void GetGenericInterfaces_ShouldThrowArgumentException_WhenInterfaceTypeIsNotGeneric()
  {
    // Arrange

    // Act
    Func<IEnumerable<Type>> act = () => typeof(StubClass0).GetGenericInterfaces(typeof(INonGenericInterface));

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }

  [Fact]
  public void GetGenericInterfaces_ShouldThrowArgumentException_WhenInterfaceTypeIsNotInterface()
  {
    // Arrange

    // Act
    Func<IEnumerable<Type>> act = () => typeof(StubClass0).GetGenericInterfaces(typeof(StubClass0));

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }

  private interface INonGenericInterface { }

  private interface IGenericInterface<T> { }

  private class StubClass0 : INonGenericInterface { }

  private class StubClass1 : IGenericInterface<int>, INonGenericInterface { }

  private class StubClass2 : IGenericInterface<int>, IGenericInterface<string>, INonGenericInterface { }
}
