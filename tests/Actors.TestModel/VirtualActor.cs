using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

public interface IVirtualActor
{
}

[Actor, Virtual]
public class VirtualActor : IVirtualActor
{
  [State]
  private readonly ComplexObject _obj;
  
  [State(Default = VirtualActorFixture.State1Default)]
  private readonly string _state1;
  
  [State]
  private int _state2;

  public VirtualActor(ComplexObject obj, string state1, int state2)
  {
    _obj = obj;
    _state1 = state1;
    _state2 = state2;
  }

  public void IncrementState2()
  {
    _state2++;
  }

  public void IncrementField0()
  {
    _obj.Field0++;
  }
}

public class ComplexObject
{
  public int Field0 { get; set; }
  public string Field1 { get; set; } = "field1";

  public override bool Equals(object? obj)
  {
    if (obj is not ComplexObject other)
      return false;

    return Field0 == other.Field0 && Field1 == other.Field1;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Field0, Field1);
  }
}

internal class VirtualActorState : OrdinaryActorState
{
  public ComplexObject _0 { get; set; } = default!;
  public string _1 { get; set; } = default!;
  public int _2 { get; set; }

  public override bool Equals(object? obj)
  {
    if (obj is not VirtualActorState other)
      return false;

    return (_0, _1, _2).Equals((other._0, other._1, other._2));
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(_0, _1, _2);
  }
}

internal abstract class VirtualActorActivity : Activity<VirtualActor>
{
}

internal class VirtualActorActivity1 : VirtualActorActivity
{
  public override int Id => 1;

  public override Task ExecuteAsync(VirtualActor actor, CancellationToken cancellationToken)
  {
    actor.IncrementState2();
    return Task.CompletedTask;
  }
}

internal class VirtualActorActivity2 : VirtualActorActivity
{
  public override int Id => 2;

  public override Task ExecuteAsync(VirtualActor actor, CancellationToken cancellationToken)
  {
    actor.IncrementField0();
    return Task.CompletedTask;
  }
}

[ActorFactory(typeof(VirtualActor))]
internal interface IVirtualActorFactory
{
  IVirtualActor Get(string id);
}

internal class VirtualActorDescriptorFactory : ActorDescriptorFactory<VirtualActor>
{
  public VirtualActorDescriptorFactory(IStateTypeBuilder stateTypeBuilder, IActivityTypeBuilder activityTypeBuilder)
    : base(stateTypeBuilder, activityTypeBuilder)
  {
  }

  protected override Type? InterfaceType => typeof(IVirtualActor);

  protected override Type? FactoryType => typeof(IVirtualActorFactory);

  protected override Type? IdType => typeof(string);

  protected override bool IsExplicitVirtual => true;

  protected override IReadOnlyCollection<StateComponentMetadata<VirtualActor>> StateComponents => new VirtualActorStateComponentMetadata[]
  {
    new VirtualActorStateComponentMetadata(0, VirtualActorFixture.ObjField, typeof(ComplexObject), new ComplexObject()),
    new VirtualActorStateComponentMetadata(1, VirtualActorFixture.State1Field, typeof(string), VirtualActorFixture.State1Default),
    new VirtualActorStateComponentMetadata(2, VirtualActorFixture.State2Field, typeof(int), 0),
  };

  protected override IEnumerable<MemberMetadata> ActorIdMembers => Enumerable.Empty<MemberMetadata>();

  protected override ImplementationDescriptorFactory<VirtualActor> BaseImplementationFactory => new VirtualActorImplementationDescriptorFactory(0, this);

  protected override IReadOnlyCollection<ImplementationDescriptorFactory<VirtualActor>> ImplementationFactories => Array.Empty<ImplementationDescriptorFactory<VirtualActor>>();

  protected override IReadOnlyCollection<IMessageHandlerMetadata> GetMessageHandlerMetadataCollection(IMethodDescriptor activity)
  {
    throw new NotImplementedException();
  }
}

internal class VirtualActorStateComponentMetadata : StateComponentMetadata<VirtualActor>
{
  private readonly object _defaultValue;

  public VirtualActorStateComponentMetadata(int index, MemberInfo member, Type type, object defaultValue)
    : base(index, member, type)
  {
    _defaultValue = defaultValue;
  }

  public override bool IsActorId => false;

  public override bool HasDefaultValue(out object? defaultComponentValue)
  {
    defaultComponentValue = _defaultValue;
    return true;
  }
}

internal class VirtualActorImplementationDescriptorFactory : ImplementationDescriptorFactory<VirtualActor>
{
  public VirtualActorImplementationDescriptorFactory(int id, ActorDescriptorFactory<VirtualActor> actorDescriptorFactory)
    : base(id, actorDescriptorFactory)
  {
  }

  public override bool IsDefault => true;

  public override Type ImplementationType => typeof(VirtualActor);

  protected override bool DefinesStateMembers => true;

  protected override ConstructorInfo? Constructor => VirtualActorFixture.Constructor;
}

internal static class VirtualActorFixture
{
  public const string State1Default = "state1Default";

  public static IActorDescriptor Descriptor;
  public static readonly ConstructorInfo Constructor;
  public static readonly FieldInfo ObjField;
  public static readonly FieldInfo State1Field;
  public static readonly FieldInfo State2Field;

  static VirtualActorFixture()
  {
    Constructor = typeof(VirtualActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { typeof(ComplexObject), typeof(string), typeof(int) });

    ObjField = typeof(VirtualActor).GetRequiredField(
      name: "_obj",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    State1Field = typeof(VirtualActor).GetRequiredField(
      name: "_state1",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    State2Field = typeof(VirtualActor).GetRequiredField(
      name: "_state2",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    MethodInfo factoryGetMethod = typeof(IVirtualActorFactory).GetRequiredMethod(
      name: nameof(IVirtualActorFactory.Get),
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { typeof(string) });

    FieldInfo[] stateFields = typeof(VirtualActorState).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);


    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(VirtualActor)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IVirtualActor))
      .SetActorType(typeof(VirtualActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetIsVirtual(true)
      .SetActivityBaseType(typeof(VirtualActorActivity))
      .SetMethods()
      .SetActivities()
      .SetId(_ => _
        .SetType(typeof(string))
        .SetHasIdSource(false)
        .SetStateIndex(-1))
      .SetState(_ => _
        .SetType(new StateTypeDelegator(typeof(VirtualActorState)))
        .SetFields(stateFields)
        .SetDefaultValue(new VirtualActorState
        {
          _0 = new ComplexObject(),
          _1 = State1Default,
          _2 = 0
        }))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IVirtualActorFactory))
        .SetCreateAsyncMethod(null)
        .SetGetMethod(factoryGetMethod))
      .SetMessageHandlers());
  }

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(VirtualActor);
    Type activityBaseType = typeof(VirtualActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), false))
      .Returns(Descriptor.State.Type);

    activityTypeBuilderMock
      .Setup(x => x.BuildBase(actorType))
      .Returns(activityBaseType);
  }

  public static void AssertValidDescriptor(IActorDescriptor descriptor)
  {
    descriptor.Should().BeAssignableTo<IActorDescriptor<VirtualActor, VirtualActorState>>();
    descriptor.Should().BeEquivalentTo(Descriptor, opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
  }
}
