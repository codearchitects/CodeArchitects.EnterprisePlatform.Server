using CodeArchitects.Platform.Actors.Fixtures.Examples;
using CodeArchitects.Platform.Actors.Metadata.Implementation;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public class ActorMetadataBuilderTests
{
  [Fact]
  public void It_ShouldBuildMetadata_ForStandardActor()
  {
    // Arrange
    ActorMetadataBuilder<StandardActor> sut = new();

    // Act
    sut.AsBuilder()
      .HasInterfaceType<IStandardActor>()
      .HasFactoryType<IStandardActorFactory>()
      .HasActorConstructor(arg => new StandardActor(
        arg.State<string>(),
        arg.Default<IService1>(),
        arg.State<StandardActorStateComponent>(),
        arg.Default<IActorContext<StandardActor>>(),
        arg.Optional<IService2>()))
      .HasMethod((actor, arg) => actor.ValueTaskTMethod(), method => method
        .IsStateless());

    // Assert
    StandardActorFixture.AssertValidMetadata(sut);
  }

  [Fact]
  public void It_ShouldBuildMetadata_ForStatelessActor()
  {
    // Arrange
    ActorMetadataBuilder<StatelessActor> sut = new();

    // Act
    sut.AsBuilder()
      .HasFactoryType<IStatelessActorFactory>()
      .HasActorConstructor(arg => new StatelessActor(
        arg.Default<IService1>()));

    // Assert
    StatelessActorFixture.AssertValidMetadata(sut, true);
  }

  [Fact]
  public void It_ShouldBuildMetadata_ForVirtualActor()
  {
    // Arrange
    ActorMetadataBuilder<VirtualActor> sut = new();

    // Act
    sut.AsBuilder()
      .HasFactoryType<IVirtualActorFactory>()
      .IsVirtual()
      .HasActorConstructor(arg => new VirtualActor(
        arg.State<ComplexObject>(),
        arg.State<string>(state => state
          .HasDefaultValue(VirtualActorFixture.State1Default)),
        arg.State<int>()));

    // Assert
    VirtualActorFixture.AssertValidMetadata(sut, true);
  }

  [Fact]
  public void It_ShouldBuildMetadata_ForComponentIdSourceActor()
  {
    // Arrange
    ActorMetadataBuilder<ComponentIdSourceActor> sut = new();

    // Act
    sut.AsBuilder()
      .HasFactoryType<IComponentIdSourceActorFactory>()
      .HasActorConstructor(arg => new ComponentIdSourceActor(
        arg.State<int>(state => state
          .IsActorId())));

    // Assert
    ComponentIdSourceActorFixture.AssertValidMetadata(sut, true);
  }

  [Fact]
  public void It_ShouldBuildMetadata_ForPropertyIdSourceActor()
  {
    // Arrange
    ActorMetadataBuilder<PropertyIdSourceActor> sut = new();

    // Act
    sut.AsBuilder()
      .HasFactoryType<IPropertyIdSourceActorFactory>()
      .HasActorConstructor(arg => new PropertyIdSourceActor(
        arg.State<PropertyIdSourceActorStateComponent>(state => state
          .IsActorIdSource(component => component.Id))));

    // Assert
    PropertyIdSourceActorFixture.AssertValidMetadata(sut, true);
  }
}
