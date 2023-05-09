using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public class ActorConfigurationTests
{
  [Fact]
  public void CreateModel_ShouldCreateCorrectActorModel()
  {
    // Arrange
    TestActorConfiguration sut = new();
    sut.Configure();

    // Act
    IActorModel model = sut.CreateModel(DynamicAssembly.NewModule());

    // Assert
    model.Actors.Should().HaveCount(5)
      .And.ContainSingle(actor => actor.ActorType == typeof(StandardActor))
      .And.ContainSingle(actor => actor.ActorType == typeof(PolymorphicActor))
      .And.ContainSingle(actor => actor.ActorType == typeof(StatelessActor))
      .And.ContainSingle(actor => actor.ActorType == typeof(VirtualActor))
      .And.ContainSingle(actor => actor.ActorType == typeof(StateIdActor));
  }

  private class TestActorConfiguration : ActorConfiguration
  {
    protected internal override void Configure()
    {
      Actor<StandardActor>(actor => actor
        .HasInterfaceType<IStandardActor>()
        .HasFactoryType<IStandardActorFactory>()
        .HasState("_state1")
        .HasState("_state2")
        .HasConstructor(arg => new StandardActor(
          arg.OfType<string>(),
          arg.OfType<IService1>(),
          arg.OfType<StandardActorStateComponent>(),
          arg.Context(),
          arg.OfType<IService2?>())));

      Actor<PolymorphicActor>(actor => actor
        .HasInterfaceType<IPolymorphicActor>()
        .HasFactoryType<IPolymorphicActorFactory>()
        .HasConstructor(typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor>))
        .HasImplementation<PolymorphicActorImplementation1>(implementation => implementation
          .HasConstructor(arg => new PolymorphicActorImplementation1(
            arg.OfType<int>(),
            arg.OfType<IService1>(),
            arg.Context())))
        .HasImplementation<PolymorphicActorImplementation2>(implementation => implementation
          .IsDefault())
        .HasState("_state"));

      Actor<StatelessActor>(actor => actor
        .HasFactoryType<IStatelessActorFactory>());

      Actor<VirtualActor>(actor => actor
        .HasFactoryType<IVirtualActorFactory>()
        .IsVirtual()
        .HasState("_obj")
        .HasState<string>("_state1", state => state
          .HasDefaultValue(VirtualActorFixture.State1Default))
        .HasState("_state2"));

      Actor<StateIdActor>(actor => actor
        .HasFactoryType<IStateIdActorFactory>()
        .HasState<int>("_state", state => state
          .IsActorId()));
    }
  }
}
