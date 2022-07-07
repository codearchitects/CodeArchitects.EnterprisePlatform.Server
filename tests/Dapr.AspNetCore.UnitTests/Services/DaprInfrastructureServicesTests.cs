using static CodeArchitects.Platform.Dapr.AspNetCore.Services.DaprInfrastructureServicesFixture;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Services;

public class DaprInfrastructureServicesTests
{
  private readonly Dictionary<Type, object> _services;

  private readonly DaprInfrastructureServices _sut;

  public DaprInfrastructureServicesTests()
  {
    _services = new();

    _sut = new(_services);
  }

  [Fact]
  public void AddService_ShouldAddServiceToDictionary()
  {
    // Arrange
    MyService service = new MyService();

    // Act
    _sut.AddService<IMyService>(service);

    // Assert
    _services.Should().HaveCount(1).And.Contain(KeyValuePair.Create(typeof(IMyService), service as object));
  }

  [Fact]
  public void GetService_ShouldReturnService_WhenServiceWasAdded()
  {
    // Arrange
    MyService expected = new MyService();
    _services.Add(typeof(IMyService), expected);

    // Act
    IMyService actual = _sut.GetService<IMyService>();

    // Assert
    actual.Should().BeSameAs(expected);
  }

  [Fact]
  public void GetService_ShouldThrowInvalidOperationException_WhenServiceWasNotAdded()
  {
    // Arrange

    // Act
    Func<IMyService> act = () => _sut.GetService<IMyService>();

    // Assert
    act.Should().ThrowExactly<InvalidOperationException>();
  }
}
