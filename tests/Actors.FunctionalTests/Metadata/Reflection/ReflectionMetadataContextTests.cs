using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

public class ReflectionMetadataContextTests
{
  [Fact]
  public void CreateModel_ShouldCreateCorrectActorModel()
  {
    // Arrange
    ReflectionMetadataContext sut = new();
    sut.AddAssembly(typeof(StandardActor).Assembly);

    // Act
    IActorModel model = sut.CreateModel(DynamicAssembly.NewModule());

    // Assert
    model.Actors.Should().HaveCount(5)
      .And.ContainSingle(actor => actor.ActorType == typeof(StandardActor))
      .And.ContainSingle(actor => actor.ActorType == typeof(PolymorphicActor))
      .And.ContainSingle(actor => actor.ActorType == typeof(StatelessActor))
      .And.ContainSingle(actor => actor.ActorType == typeof(VirtualActor))
      .And.ContainSingle(actor => actor.ActorType == typeof(ComponentIdSourceActor));
  }
}
