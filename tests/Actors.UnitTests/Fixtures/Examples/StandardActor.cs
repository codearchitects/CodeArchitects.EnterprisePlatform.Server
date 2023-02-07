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

  public void VoidActivity()
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

internal abstract class StandardActorActivity : Activity<StandardActor>
{
}

internal class StandardActorActivity1 : StandardActorActivity
{
  public override int Id => 1;

  public int arg { get; set; }

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => throw new NotImplementedException();
}

internal class StandardActorActivity2 : StandardActorActivity
{
  public override int Id => 2;

  public int arg { get; set; }

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => throw new NotImplementedException();
}

internal class StandardActorActivity3 : StandardActorActivity
{
  public override int Id => 3;

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => throw new NotImplementedException();
}

internal class StandardActorActivity4 : StandardActorActivity
{
  public override int Id => 4;

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => throw new NotImplementedException();
}

internal class StandardActorActivity5 : StandardActorActivity
{
  public override int Id => 5;

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => throw new NotImplementedException();
}

internal class StandardActorActivity6 : StandardActorActivity
{
  public override int Id => 6;

  public int arg { get; set; }

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => throw new NotImplementedException();
}

internal class StandardActorActivity7 : StandardActorActivity
{
  public override int Id => 7;

  public string arg { get; set; }

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => throw new NotImplementedException();
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

    MethodInfo implementationTaskMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.TaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    MethodInfo implementationTaskTMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.TaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(CancellationToken) });

    MethodInfo implementationValueTaskMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.ValueTaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(CancellationToken) });

    MethodInfo implementationValueTaskTMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.ValueTaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo voidActivityMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.VoidActivity),
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

    FieldInfo[] activity1Fields = typeof(StandardActorActivity1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] activity2Fields = typeof(StandardActorActivity2).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] activity3Fields = typeof(StandardActorActivity3).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] activity4Fields = typeof(StandardActorActivity4).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] activity5Fields = typeof(StandardActorActivity5).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] activity6Fields = typeof(StandardActorActivity6).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] activity7Fields = typeof(StandardActorActivity7).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);


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
      .SetImplementationMethod(implementationTaskMethod)
      .SetParameterTypes(typeof(int))
      .SetActivityType(typeof(StandardActorActivity1))
      .SetActivityFields(activity1Fields)
      .SetHasCancellationTokenParameter(false)
      .SetCancellationTokenParameterPosition(-1));

    ITaskTMethodDescriptor taskTMethod = TaskTMethodDescriptorBuilder.Build(_ => _
      .InitDefaults(typeof(int))
      .SetId(2)
      .SetImplementationMethod(implementationTaskTMethod)
      .SetParameterTypes(typeof(int), typeof(CancellationToken))
      .SetActivityType(typeof(StandardActorActivity2))
      .SetActivityFields(activity2Fields)
      .SetHasCancellationTokenParameter(true)
      .SetCancellationTokenParameterPosition(1));

    IValueTaskMethodDescriptor valueTaskMethod = ValueTaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(3)
      .SetImplementationMethod(implementationValueTaskMethod)
      .SetParameterTypes(typeof(CancellationToken))
      .SetActivityType(typeof(StandardActorActivity3))
      .SetActivityFields(activity3Fields)
      .SetHasCancellationTokenParameter(true)
      .SetCancellationTokenParameterPosition(0));

    IValueTaskTMethodDescriptor valueTaskTMethod = ValueTaskTMethodDescriptorBuilder.Build(_ => _
      .InitDefaults(typeof(string))
      .SetId(4)
      .SetImplementationMethod(implementationValueTaskTMethod)
      .SetParameterTypes()
      .SetActivityType(typeof(StandardActorActivity4))
      .SetActivityFields(activity4Fields)
      .SetHasCancellationTokenParameter(false)
      .SetCancellationTokenParameterPosition(-1));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
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
        .Add<VoidMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(5)
          .SetImplementationMethod(voidActivityMethod)
          .SetParameterTypes()
          .SetActivityType(typeof(StandardActorActivity5))
          .SetActivityFields(activity1Fields)
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(6)
          .SetImplementationMethod(activityOverload1Method)
          .SetParameterTypes(typeof(int))
          .SetActivityType(typeof(StandardActorActivity6))
          .SetActivityFields(activity6Fields)
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(7)
          .SetImplementationMethod(activityOverload2Method)
          .SetParameterTypes(typeof(string))
          .SetActivityType(typeof(StandardActorActivity7))
          .SetActivityFields(activity7Fields)
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
