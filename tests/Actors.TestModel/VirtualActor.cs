using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Infrastructure;
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
  private readonly int _state2;

  public VirtualActor(ComplexObject obj, string state1, int state2)
  {
    _obj = obj;
    _state1 = state1;
    _state2 = state2;
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

[ActorFactory(typeof(VirtualActor))]
internal interface IVirtualActorFactory
{
  IVirtualActor Get(string id);
}

internal static class VirtualActorFixture
{
  public const string State1Default = "state1Default";

  public static IActorDescriptor Descriptor;

  static VirtualActorFixture()
  {
    ConstructorInfo constructor = typeof(VirtualActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { typeof(ComplexObject), typeof(string), typeof(int) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

    FieldInfo objField = typeof(VirtualActor).GetRequiredField(
      name: "_obj",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    FieldInfo state1Field = typeof(VirtualActor).GetRequiredField(
      name: "_state1",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    FieldInfo state2Field = typeof(VirtualActor).GetRequiredField(
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
        .SetStateIndex(-1)
        .SetIdProperty(null))
      .SetState(_ => _
        .SetType(typeof(VirtualActorState))
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
        .SetGetMethod(factoryGetMethod)));
  }

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(VirtualActor);
    Type activityBaseType = typeof(VirtualActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), false))
      .Returns(typeof(VirtualActorState));

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
