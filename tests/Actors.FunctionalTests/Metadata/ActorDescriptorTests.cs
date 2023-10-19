using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Actors.TestModel;
using Moq;

namespace CodeArchitects.Platform.Actors.Metadata;

public class ActorDescriptorTests
{
  private readonly IActorDescriptor<VirtualActor, VirtualActorState> _descriptor;

  public ActorDescriptorTests()
  {
    Mock<IStateTypeBuilder> stateTypeBuilderMock = new(MockBehavior.Strict);
    stateTypeBuilderMock
      .Setup(x => x.Build(typeof(VirtualActor), It.Is<IEnumerable<IStateComponentMetadata>>(components => components.Count() == 3), false))
      .Returns(typeof(VirtualActorState));

    Mock<IActivityTypeBuilder> activityTypeBuilderMock = new(MockBehavior.Strict);
    activityTypeBuilderMock
      .Setup(x => x.BuildBase(typeof(VirtualActor)))
      .Returns(typeof(VirtualActorActivity));
    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Name == nameof(VirtualActor.IncrementState2)), typeof(VirtualActor), typeof(VirtualActorActivity)))
      .Returns(typeof(VirtualActorActivity1));
    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Name == nameof(VirtualActor.IncrementField0)), typeof(VirtualActor), typeof(VirtualActorActivity)))
      .Returns(typeof(VirtualActorActivity2));

    VirtualActorDescriptorFactory factory = new VirtualActorDescriptorFactory(stateTypeBuilderMock.Object, activityTypeBuilderMock.Object);
    _descriptor = (IActorDescriptor<VirtualActor, VirtualActorState>)factory.CreateDescriptor();
  }

  [Fact]
  public void UpdateState_ShouldNotModifyVirtualStateDefaultValue_WhenStateComponentIsValueType()
  {
    // Arrange
    VirtualActorState defaultState = _descriptor.State.DefaultValue!;
    VirtualActorState expected = new VirtualActorState
    {
      _0 = new ComplexObject
      {
        Field0 = defaultState._0.Field0,
        Field1 = defaultState._0.Field1
      },
      _1 = defaultState._1,
      _2 = defaultState._2
    };

    // Act
    VirtualActor actor = _descriptor.CreateInstance(0, Mock.Of<IServiceProvider>(), defaultState, Mock.Of<IActorContext<VirtualActor>>());
    actor.IncrementState2();
    _descriptor.UpdateState(actor, defaultState);

    // Assert
    _descriptor.State.DefaultValue!.Should().Be(expected);
  }

  [Fact]
  public void UpdateState_ShouldNotModifyVirtualStateDefaultValue_WhenStateComponentIsReferenceType()
  {
    // Arrange
    VirtualActorState defaultState = _descriptor.State.DefaultValue!;
    VirtualActorState expected = new VirtualActorState
    {
      _0 = new ComplexObject
      {
        Field0 = defaultState._0.Field0,
        Field1 = defaultState._0.Field1
      },
      _1 = defaultState._1,
      _2 = defaultState._2
    };

    // Act
    VirtualActor actor = _descriptor.CreateInstance(0, Mock.Of<IServiceProvider>(), defaultState, Mock.Of<IActorContext<VirtualActor>>());
    actor.IncrementField0();
    _descriptor.UpdateState(actor, defaultState);

    // Assert
    _descriptor.State.DefaultValue!.Should().Be(expected);
  }
}
