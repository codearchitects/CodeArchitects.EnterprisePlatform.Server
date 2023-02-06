using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures.Examples;

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

internal class VirtualActorState
{
  public ComplexObject _obj { get; set; } = default!;
  public string _state1 { get; set; } = default!;
  public int _state2 { get; set; }
}

[ActorFactory(typeof(VirtualActor))]
public interface IVirtualActorFactory
{
  IVirtualActor Get(string id);
}

internal static class VirtualActorFixture
{
  public const string State1Default = "state1Default";

  public static IActorDescriptor Descriptor;

  private static readonly ConstructorInfo s_constructor;
  private static readonly FieldInfo s_objField;
  private static readonly FieldInfo s_state1Field;
  private static readonly FieldInfo s_state2Field;

  static VirtualActorFixture()
  {
    s_constructor = typeof(VirtualActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { typeof(ComplexObject), typeof(string), typeof(int) });

    ParameterInfo[] constructorParameters = s_constructor.GetParameters();

    s_objField = typeof(VirtualActor).GetRequiredField(
      name: "_obj",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    s_state1Field = typeof(VirtualActor).GetRequiredField(
      name: "_state1",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    s_state2Field = typeof(VirtualActor).GetRequiredField(
      name: "_state2",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    MethodInfo factoryGetMethod = typeof(IVirtualActorFactory).GetRequiredMethod(
      name: nameof(IVirtualActorFactory.Get),
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { typeof(string) });


    IStateDependencyDescriptor objDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("obj")
      .SetType(typeof(ComplexObject))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(s_objField));

    IStateDependencyDescriptor state1Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[1])
      .SetName("state1")
      .SetType(typeof(string))
      .SetIndex(1)
      .SetFieldIndex(1)
      .SetField(s_state1Field));

    IStateDependencyDescriptor state2Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[2])
      .SetName("state2")
      .SetType(typeof(int))
      .SetIndex(2)
      .SetFieldIndex(2)
      .SetField(s_state2Field));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(VirtualActor))
      .SetConstructor(_ => _
        .SetConstructor(s_constructor)
        .SetDependencies(objDependency, state1Dependency, state2Dependency)
        .SetContextDependencies()
        .SetServiceDependencies()
        .SetStateDependencies(objDependency, state1Dependency, state2Dependency))
      .SetMethods());

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IVirtualActor))
      .SetActorType(typeof(VirtualActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetIsStateless(false)
      .SetIsVirtual(true)
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetType(typeof(VirtualActorState))
        .SetStateFields(s_objField, s_state1Field, s_state2Field)
        .SetDiscriminatorField(null)
        .SetDefaultValue(new VirtualActorState
        {
          _obj = new ComplexObject(),
          _state1 = State1Default,
          _state2 = 0
        }))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IVirtualActorFactory))
        .SetCreateAsyncMethod(null)
        .SetGetMethod(factoryGetMethod)));
  }
}
