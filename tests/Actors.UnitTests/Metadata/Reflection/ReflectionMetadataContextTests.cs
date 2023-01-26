using CodeArchitects.Platform.Actors.Fixtures.Examples;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

public class ReflectionMetadataContextTests
{
  private readonly ReflectionMetadataContext _sut;

  public ReflectionMetadataContextTests()
  {
    _sut = new();
  }

  [Fact]
  public void AddAssembly_ShouldRegisterActorTypesInAssembly()
  {
    // Arrange
    Assembly assembly = Mock.Of<Assembly>(mock => mock.GetTypes() == new[]
    {
      typeof(StandardActor), typeof(IStandardActor), typeof(IStandardActorFactory),
      typeof(StatelessActor), typeof(IStatelessActor), typeof(IStatelessActorFactory)
    }, MockBehavior.Strict);

    // Act
    _sut.AddAssembly(assembly);

    // Assert
    _sut.MetadataSources.Should().HaveCount(2)
      .And.ContainSingle(source =>
        source.ActorType == typeof(StandardActor) &&
        source.FactoryType == typeof(IStandardActorFactory) &&
        source.ActorAttribute.GetType() == typeof(ActorAttribute<IStandardActor>) &&
        source.ImplementationTypes.Count == 0)
      .And.ContainSingle(source =>
        source.ActorType == typeof(StatelessActor) &&
        source.FactoryType == typeof(IStatelessActorFactory) &&
        source.ActorAttribute.GetType() == typeof(ActorAttribute) &&
        source.ImplementationTypes.Count == 0);
  }

  [Fact]
  public void AddActor_ShouldRegisterActorType()
  {
    // Arrange

    // Act
    _sut.AddActor(typeof(VirtualActor), typeof(IVirtualActorFactory), Array.Empty<Type>());

    // Assert
    _sut.MetadataSources.Should().HaveCount(1)
      .And.ContainSingle(source =>
        source.ActorType == typeof(VirtualActor) &&
        source.FactoryType == typeof(IVirtualActorFactory) &&
        source.ActorAttribute.GetType() == typeof(ActorAttribute) &&
        source.ImplementationTypes.Count == 0);
  }
}
