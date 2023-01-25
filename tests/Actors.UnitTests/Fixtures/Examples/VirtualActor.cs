using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
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

[ActorFactory<VirtualActor>]
public interface IVirtualActorFactory
{
  IVirtualActor Get(string id);
}

internal static class VirtualActorFixture
{
  public const string State1Default = "state1Default";
  public static IActorDescriptor Descriptor;
  public static IActorMetadata Metadata;

  static VirtualActorFixture()
  {
    ConstructorInfo constructorInfo = typeof(VirtualActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { typeof(ComplexObject), typeof(string), typeof(int) });

    ParameterInfo[] constructorParameters = constructorInfo.GetParameters();

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


    IStateDependencyDescriptor objDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("obj")
      .SetType(typeof(ComplexObject))
      .SetIndex(0)
      .SetCategoryIndex(0)
      .SetField(objField));

    IStateDependencyDescriptor state1Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[1])
      .SetName("state1")
      .SetType(typeof(string))
      .SetIndex(1)
      .SetCategoryIndex(1)
      .SetField(state1Field));

    IStateDependencyDescriptor state2Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[2])
      .SetName("state2")
      .SetType(typeof(int))
      .SetIndex(2)
      .SetCategoryIndex(2)
      .SetField(state2Field));

    IConstructorDescriptor constructor = ConstructorDescriptorBuilder.Build(_ => _
      .SetConstructor(constructorInfo)
      .SetDependencies(objDependency, state1Dependency, state2Dependency)
      .SetContextDependency(null as IContextDependencyDescriptor)
      .SetServiceDependencies()
      .SetStateDependencies(objDependency, state1Dependency, state2Dependency));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(VirtualActor))
      .SetConstructor(constructor)
      .SetMethods());

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IVirtualActor))
      .SetActorType(typeof(VirtualActor))
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetStateType(typeof(VirtualActorState))
        .SetIsStateless(false)
        .SetIsVirtual(true)
        .SetFields(objField, state1Field, state2Field)
        .SetDefaultValues(new ComplexObject(), State1Default, 0))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IVirtualActorFactory))
        .SetCreateAsyncMethod(null)
        .SetGetMethod(factoryGetMethod))
      .SetConstructor(constructor));


    Metadata = ActorMetadataBuilder.Build(_ => _
      .SetInterfaceType(typeof(IVirtualActor))
      .SetActorType(typeof(VirtualActor))
      .SetIsExplicitVirtual(true)
      .SetFactoryType(typeof(IVirtualActorFactory))
      .SetConstructor(constructorInfo)
      .SetStateFields(_ => _
        .Add(_ => _
          .SetField(objField)
          .SetDefaultValue(new ComplexObject())
          .SetIsActorIdSource(false))
        .Add(_ => _
          .SetField(state1Field)
          .SetDefaultValue(State1Default)
          .SetIsActorIdSource(false))
        .Add(_ => _
          .SetField(state2Field)
          .SetDefaultValue(0)
          .SetIsActorIdSource(false)))
      .SetImplementations(_ => _
        .Add(_ => _
          .SetIsDefault(true)
          .SetImplementationType(typeof(VirtualActor))
          .SetConstructor(constructorInfo))));
  }
}
