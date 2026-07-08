using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Actors.TestModel;
using Moq;

namespace CodeArchitects.Platform.Actors.Metadata;

public class ActorDescriptorTests
{
  private readonly Mock<IStateTypeBuilder> _stateTypeBuilderMock;
  private readonly Mock<IActivityTypeBuilder> _activityTypeBuilderMock;

  public ActorDescriptorTests()
  {
    _stateTypeBuilderMock = new(MockBehavior.Strict);
    _stateTypeBuilderMock
      .Setup(x => x.Build(typeof(VirtualActor), It.Is<IEnumerable<IStateComponentMetadata>>(components => components.Count() == 3), false))
      .Returns(typeof(VirtualActorState));

    _activityTypeBuilderMock = new(MockBehavior.Strict);
    _activityTypeBuilderMock
      .Setup(x => x.BuildBase(typeof(VirtualActor)))
      .Returns(typeof(VirtualActorActivity));
    _activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 1), typeof(VirtualActor), typeof(VirtualActorActivity)))
      .Returns(typeof(VirtualActorActivity1));
    _activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 2), typeof(VirtualActor), typeof(VirtualActorActivity)))
      .Returns(typeof(VirtualActorActivity2));
  }

  private VirtualActorDescriptorFactory CreateDescriptorFactory(params StateComponentMetadata<VirtualActor>[] components)
  {
    return new VirtualActorDescriptorFactory(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object, components);
  }

  [Fact]
  public void UpdateState_ShouldNotModifyVirtualStateDefaultValue_WhenStateComponentIsValueType()
  {
    // Arrange
    VirtualActorDescriptorFactory factory = CreateDescriptorFactory(
      new VirtualActorStateComponentMetadataWithFactory<ComplexObject>(0, VirtualActorFixture.ObjField, typeof(ComplexObject), () => new ComplexObject()),
      new VirtualActorStateComponentMetadataWithConstant<string>(1, VirtualActorFixture.State1Field, typeof(string), VirtualActorFixture.State1Default),
      new VirtualActorStateComponentMetadataWithConstant<int>(2, VirtualActorFixture.State2Field, typeof(int), 0));

    IActorDescriptor<VirtualActor, VirtualActorState>  descriptor = (IActorDescriptor<VirtualActor, VirtualActorState>)factory.CreateDescriptor();
    VirtualActorState defaultState = descriptor.State.GetDefaultValue();
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
    VirtualActor actor = descriptor.CreateInstance(0, Mock.Of<IServiceProvider>(), defaultState, Mock.Of<IActorContext<VirtualActor>>());
    actor.IncrementState2();
    descriptor.UpdateState(actor, defaultState);

    // Assert
    descriptor.State.GetDefaultValue().Should().Be(expected);
  }

  [Fact]
  public void UpdateState_ShouldNotModifyVirtualStateDefaultValue_WhenStateComponentIsReferenceTypeAndDefaultValueFactoryIsUsed()
  {
    // Arrange
    VirtualActorDescriptorFactory factory = CreateDescriptorFactory(
      new VirtualActorStateComponentMetadataWithFactory<ComplexObject>(0, VirtualActorFixture.ObjField, typeof(ComplexObject), () => new ComplexObject()),
      new VirtualActorStateComponentMetadataWithConstant<string>(1, VirtualActorFixture.State1Field, typeof(string), VirtualActorFixture.State1Default),
      new VirtualActorStateComponentMetadataWithConstant<int>(2, VirtualActorFixture.State2Field, typeof(int), 0));

    IActorDescriptor<VirtualActor, VirtualActorState> descriptor = (IActorDescriptor<VirtualActor, VirtualActorState>)factory.CreateDescriptor();
    VirtualActorState defaultState = descriptor.State.GetDefaultValue();
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
    VirtualActor actor = descriptor.CreateInstance(0, Mock.Of<IServiceProvider>(), defaultState, Mock.Of<IActorContext<VirtualActor>>());
    actor.IncrementField0();
    descriptor.UpdateState(actor, defaultState);

    // Assert
    descriptor.State.GetDefaultValue().Should().Be(expected);
  }

  [Fact]
  public void UpdateState_ShouldModifyVirtualStateDefaultValue_WhenStateComponentIsReferenceTypeAndDefaultValueConstantIsUsed()
  {
    // Arrange
    VirtualActorDescriptorFactory factory = CreateDescriptorFactory(
      new VirtualActorStateComponentMetadataWithConstant<ComplexObject>(0, VirtualActorFixture.ObjField, typeof(ComplexObject), new ComplexObject()),
      new VirtualActorStateComponentMetadataWithConstant<string>(1, VirtualActorFixture.State1Field, typeof(string), VirtualActorFixture.State1Default),
      new VirtualActorStateComponentMetadataWithConstant<int>(2, VirtualActorFixture.State2Field, typeof(int), 0));

    IActorDescriptor<VirtualActor, VirtualActorState> descriptor = (IActorDescriptor<VirtualActor, VirtualActorState>)factory.CreateDescriptor();
    VirtualActorState defaultState = descriptor.State.GetDefaultValue();
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
    VirtualActor actor = descriptor.CreateInstance(0, Mock.Of<IServiceProvider>(), defaultState, Mock.Of<IActorContext<VirtualActor>>());
    actor.IncrementField0();
    descriptor.UpdateState(actor, defaultState);

    // Assert
    descriptor.State.GetDefaultValue().Should().NotBe(expected);
  }
}
