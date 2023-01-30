using CodeArchitects.Platform.Actors.Fixtures.Examples;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

public class ReflectionActorMetadataTests
{
  [Fact]
  public void Create_ShouldCreateMetadata_ForStandardActor()
  {
    // Arrange
    ReflectionMetadataSource source = new(typeof(StandardActor), typeof(IStandardActorFactory), new ActorAttribute<IStandardActor>(), Array.Empty<Type>());

    // Act
    ReflectionActorMetadata metadata = ReflectionActorMetadata.Create(source);

    // Assert
    StandardActorFixture.AssertValidMetadata(metadata);
  }

  [Fact]
  public void Create_ShouldCreateMetadata_ForStatelessActor()
  {
    // Arrange
    ReflectionMetadataSource source = new(typeof(StatelessActor), typeof(IStatelessActorFactory), new ActorAttribute(), Array.Empty<Type>());

    // Act
    ReflectionActorMetadata metadata = ReflectionActorMetadata.Create(source);

    // Assert
    StatelessActorFixture.AssertValidMetadata(metadata, false);
  }

  [Fact]
  public void Create_ShouldCreateMetadata_ForVirtualActor()
  {
    // Arrange
    ReflectionMetadataSource source = new(typeof(VirtualActor), typeof(IVirtualActorFactory), new ActorAttribute(), Array.Empty<Type>());

    // Act
    ReflectionActorMetadata metadata = ReflectionActorMetadata.Create(source);

    // Assert
    VirtualActorFixture.AssertValidMetadata(metadata, false);
  }

  [Fact]
  public void Create_ShouldCreateMetadata_ForComponentIdSourceActor()
  {
    // Arrange
    ReflectionMetadataSource source = new(typeof(ComponentIdSourceActor), typeof(IComponentIdSourceActorFactory), new ActorAttribute(), Array.Empty<Type>());

    // Act
    ReflectionActorMetadata metadata = ReflectionActorMetadata.Create(source);

    // Assert
    ComponentIdSourceActorFixture.AssertValidMetadata(metadata, false);
  }

  [Fact]
  public void Create_ShouldCreateMetadata_ForPropertyIdSourceActor()
  {
    // Arrange
    ReflectionMetadataSource source = new(typeof(PropertyIdSourceActor), typeof(IPropertyIdSourceActorFactory), new ActorAttribute(), Array.Empty<Type>());

    // Act
    ReflectionActorMetadata metadata = ReflectionActorMetadata.Create(source);

    // Assert
    PropertyIdSourceActorFixture.AssertValidMetadata(metadata, false);
  }
}
