using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures.Examples;

internal class StandardActorStateComponent
{
  public override bool Equals(object? obj)
  {
    return true;
  }

  public override int GetHashCode()
  {
    return 0;
  }
}

internal interface IStandardActor
{
  Task TaskMethod(int arg);

  Task<int> TaskTMethod(int arg, CancellationToken cancellationToken);
  
  ValueTask ValueTaskMethod(CancellationToken cancellationToken);
  
  ValueTask<string> ValueTaskTMethod();
}

[Actor<IStandardActor>]
internal class StandardActor : IStandardActor, IDisposable
{
  [State] private readonly string _state1;
  private readonly IService1 _service1;
  [State] private readonly StandardActorStateComponent _state2;
  private readonly IActorContext<StandardActor> _context;
  private readonly IService2? _service2;

  [ActorConstructor]
  public StandardActor(string state1, IService1 service1, StandardActorStateComponent state2, IActorContext<StandardActor> context, IService2? service2 = null)
  {
    _state1 = state1;
    _service1 = service1;
    _state2 = state2;
    _context = context;
    _service2 = service2;
  }

  public StandardActor()
  {
    _state1 = default!;
    _service1 = default!;
    _state2 = default!;
    _context = default!;
    _service2 = default!;
  }

  public Task TaskMethod(int arg)
    => throw new NotImplementedException();

  public Task<int> TaskTMethod(int arg, CancellationToken cancellationToken)
    => throw new NotImplementedException();

  public ValueTask ValueTaskMethod(CancellationToken cancellationToken)
    => throw new NotImplementedException();

  public ValueTask<string> ValueTaskTMethod()
    => throw new NotImplementedException();

  public void Dispose()
    => throw new NotImplementedException();

  public Task ActivityOverload(int arg)
    => throw new NotImplementedException();

  public Task ActivityOverload(string arg)
    => throw new NotImplementedException();
}

[ActorFactory<StandardActor>]
internal interface IStandardActorFactory
{
  Task<IStandardActor> CreateAsync(string id, string state1, StandardActorStateComponent state2, CancellationToken cancellationToken = default);
  IStandardActor Get(string id);
}

internal class StandardActorState
{
  public string _state1 { get; set; }
  public StandardActorStateComponent _state2 { get; set; } = default!;
}

internal class TaskMethodPayload : ActivityPayload
{
  public override int ActivityId => 1;

  public int Arg { get; set; }
}

internal class TaskTMethodPayload : ActivityPayload
{
  public override int ActivityId => 2;

  public int Arg { get; set; }
}

internal class ValueTaskMethodPayload : ActivityPayload
{
  public override int ActivityId => 3;
}

internal class ValueTaskTMethodPayload : ActivityPayload
{
  public override int ActivityId => 4;
}

internal class ActivityOverload1Payload : ActivityPayload
{
  public override int ActivityId => 5;

  public int Arg { get; set; }
}

internal class ActivityOverload2Payload : ActivityPayload
{
  public override int ActivityId => 6;

  public string Arg { get; set; }
}

internal static class StandardActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  private static readonly ConstructorInfo s_constructor;
  private static readonly FieldInfo s_state1Field;
  private static readonly FieldInfo s_state2Field;

  static StandardActorFixture()
  {
    s_state1Field = typeof(StandardActor).GetRequiredField(
      name: "_state1",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    s_state2Field = typeof(StandardActor).GetRequiredField(
      name: "_state2",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    MethodInfo interfaceTaskMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.TaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    MethodInfo implementationTaskMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.TaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    MethodInfo interfaceTaskTMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.TaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(CancellationToken) });

    MethodInfo implementationTaskTMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.TaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(CancellationToken) });

    MethodInfo interfaceValueTaskMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.ValueTaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(CancellationToken) });

    MethodInfo implementationValueTaskMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.ValueTaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(CancellationToken) });

    MethodInfo interfaceValueTaskTMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.ValueTaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementationValueTaskTMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.ValueTaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo activityOverload1Method = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.ActivityOverload),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    MethodInfo activityOverload2Method = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.ActivityOverload),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    s_constructor = typeof(StandardActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string), typeof(IService1), typeof(StandardActorStateComponent), typeof(IActorContext<StandardActor>), typeof(IService2) });

    ParameterInfo[] constructorParameters = s_constructor.GetParameters();

    MethodInfo factoryCreateAsyncMethod = typeof(IStandardActorFactory).GetRequiredMethod(
      name: nameof(IStandardActorFactory.CreateAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string), typeof(string), typeof(StandardActorStateComponent), typeof(CancellationToken) });

    MethodInfo factoryGetMethod = typeof(IStandardActorFactory).GetRequiredMethod(
      name: nameof(IStandardActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    FieldInfo[] taskMethodPayloadFields = typeof(TaskMethodPayload).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] taskTMethodPayloadFields = typeof(TaskTMethodPayload).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] valueTaskMethodPayloadFields = typeof(ValueTaskMethodPayload).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] valueTaskTMethodPayloadFields = typeof(ValueTaskTMethodPayload).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] activityOverload1PayloadFields = typeof(ActivityOverload1Payload).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] activityOverload2PayloadFields = typeof(ActivityOverload2Payload).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);


    IStateDependencyDescriptor state1Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("state1")
      .SetType(typeof(string))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(s_state1Field));

    IServiceDependencyDescriptor service1Dependency = ServiceDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[1])
      .SetName("service1")
      .SetType(typeof(IService1))
      .SetIndex(1)
      .SetIsOptional(false));

    IStateDependencyDescriptor state2Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[2])
      .SetName("state2")
      .SetType(typeof(StandardActorStateComponent))
      .SetIndex(2)
      .SetFieldIndex(1)
      .SetField(s_state2Field));

    IContextDependencyDescriptor contextDependency = ContextDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[3])
      .SetName("context")
      .SetType(typeof(IActorContext<StandardActor>))
      .SetImplementationType(typeof(StandardActor))
      .SetIndex(3));

    IServiceDependencyDescriptor service2Dependency = ServiceDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[4])
      .SetName("service2")
      .SetType(typeof(IService2))
      .SetIndex(4)
      .SetIsOptional(true));

    ITaskMethodDescriptor taskMethod = TaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(1)
      .SetInterfaceMethod(interfaceTaskMethod)
      .SetImplementationMethod(implementationTaskMethod)
      .SetParameterTypes(typeof(int))
      .SetPayloadType(typeof(TaskMethodPayload))
      .SetPayloadFields(taskMethodPayloadFields)
      .SetHasCancellationTokenParameter(false)
      .SetCancellationTokenParameterPosition(-1));

    ITaskTMethodDescriptor taskTMethod = TaskTMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(2)
      .SetInterfaceMethod(interfaceTaskTMethod)
      .SetImplementationMethod(implementationTaskTMethod)
      .SetParameterTypes(typeof(int), typeof(CancellationToken))
      .SetPayloadType(typeof(TaskTMethodPayload))
      .SetPayloadFields(taskTMethodPayloadFields)
      .SetHasCancellationTokenParameter(true)
      .SetCancellationTokenParameterPosition(1)
      .SetResultType(typeof(int)));

    IValueTaskMethodDescriptor valueTaskMethod = ValueTaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(3)
      .SetInterfaceMethod(interfaceValueTaskMethod)
      .SetImplementationMethod(implementationValueTaskMethod)
      .SetParameterTypes(typeof(CancellationToken))
      .SetPayloadType(typeof(ValueTaskMethodPayload))
      .SetPayloadFields(valueTaskMethodPayloadFields)
      .SetHasCancellationTokenParameter(true)
      .SetCancellationTokenParameterPosition(0));

    IValueTaskTMethodDescriptor valueTaskTMethod = ValueTaskTMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(4)
      .SetInterfaceMethod(interfaceValueTaskTMethod)
      .SetImplementationMethod(implementationValueTaskTMethod)
      .SetParameterTypes()
      .SetPayloadType(typeof(ValueTaskTMethodPayload))
      .SetPayloadFields(valueTaskTMethodPayloadFields)
      .SetHasCancellationTokenParameter(false)
      .SetCancellationTokenParameterPosition(-1)
      .SetResultType(typeof(string)));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(StandardActor))
      .SetConstructor(_ => _
        .SetConstructor(s_constructor)
        .SetDependencies(state1Dependency, service1Dependency, state2Dependency, contextDependency, service2Dependency)
        .SetContextDependencies(contextDependency)
        .SetServiceDependencies(service1Dependency, service2Dependency)
        .SetStateDependencies(state1Dependency, state2Dependency))
      .SetMethods(taskMethod, taskTMethod, valueTaskMethod, valueTaskTMethod));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStandardActor))
      .SetActorType(typeof(StandardActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetIsStateless(false)
      .SetIsVirtual(false)
      .SetActivities(_ => _
        .Add(taskMethod)
        .Add(taskTMethod)
        .Add(valueTaskMethod)
        .Add(valueTaskTMethod)
        .Add<ActivityDescriptorBuilder>(_ => _
          .SetId(5)
          .SetImplementationMethod(activityOverload1Method)
          .SetParameterTypes(typeof(int))
          .SetPayloadType(typeof(ActivityOverload1Payload))
          .SetPayloadFields(activityOverload1PayloadFields)
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<ActivityDescriptorBuilder>(_ => _
          .SetId(6)
          .SetImplementationMethod(activityOverload2Method)
          .SetParameterTypes(typeof(string))
          .SetPayloadType(typeof(ActivityOverload2Payload))
          .SetPayloadFields(activityOverload2PayloadFields)
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1)))
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetType(typeof(StandardActorState))
        .SetStateFields(s_state1Field, s_state2Field)
        .SetDiscriminatorField(null)
        .SetDefaultValue(null))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IStandardActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));
  }
}
