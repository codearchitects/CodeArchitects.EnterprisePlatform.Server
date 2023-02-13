using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

internal interface IPolymorphicActor
{
  Task BaseMethod();

  Task VirtualMethod();

  Task AbstractMethod();
}

internal abstract class PolymorphicActor : IPolymorphicActor
{
  [State] private readonly int _state;
  private readonly IService1 _service1;
  private readonly IActorContext<PolymorphicActor> _context;

  [ActorConstructor]
  public PolymorphicActor(int state, IService1 service1, IActorContext<PolymorphicActor> context)
  {
    _state = state;
    _service1 = service1;
    _context = context;
  }

  public PolymorphicActor()
  {
    _state = default!;
    _service1 = default!;
    _context = default!;
  }

  public Task BaseMethod()
    => throw new NotImplementedException();

  public virtual Task VirtualMethod()
    => throw new NotImplementedException();

  public abstract Task AbstractMethod();
}

internal class PolymorphicActorImplementation1 : PolymorphicActor
{
  [ActorConstructor]
  public PolymorphicActorImplementation1(int state, IService1 service1, IActorContext<PolymorphicActor> context)
    : base(state, service1, context)
  {
  }

  public PolymorphicActorImplementation1()
  {
  }

  public override Task AbstractMethod()
    => throw new NotImplementedException();

  public override Task VirtualMethod()
    => throw new NotImplementedException();

  public virtual Task Activity()
    => throw new NotImplementedException();
}

[ActorImplementation(IsDefault = true)]
internal class PolymorphicActorImplementation2 : PolymorphicActor
{
  private readonly IService2 _service2;

  public PolymorphicActorImplementation2(int state, IService1 service1, IActorContext<PolymorphicActor> context, IService2 service2)
    : base(state, service1, context)
  {
    _service2 = service2;
  }

  public override Task AbstractMethod()
    => throw new NotImplementedException();
}

[ActorFactory<PolymorphicActor>]
internal interface IPolymorphicActorFactory
{
  Task<IPolymorphicActor> CreateAsync(string id, int state, CancellationToken cancellationToken = default);
  IPolymorphicActor Get(string id);
}

internal class PolymorphicActorState : Infrastructure.PolymorphicActorState
{
  public int _state { get; set; }
}

internal abstract class PolymorphicActorActivity : Activity<PolymorphicActor>
{
}

internal class PolymorphicActorActivity1 : PolymorphicActorActivity
{
  public override int Id => 1;

  public override Task ExecuteAsync(PolymorphicActor actor, CancellationToken cancellationToken)
  {
    return actor.BaseMethod();
  }
}

internal class PolymorphicActorActivity2 : PolymorphicActorActivity
{
  public override int Id => 2;

  public override Task ExecuteAsync(PolymorphicActor actor, CancellationToken cancellationToken)
    => actor.VirtualMethod();
}

internal class PolymorphicActorActivity3 : PolymorphicActorActivity
{
  public override int Id => 3;

  public override Task ExecuteAsync(PolymorphicActor actor, CancellationToken cancellationToken)
    => actor.AbstractMethod();
}

internal class PolymorphicActorActivity4 : PolymorphicActorActivity
{
  public override int Id => 4;

  public override Task ExecuteAsync(PolymorphicActor actor, CancellationToken cancellationToken)
    => ((PolymorphicActorImplementation1)actor).Activity();
}

internal static class PolymorphicActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  static PolymorphicActorFixture()
  {
    MethodInfo factoryCreateAsyncMethod = typeof(IPolymorphicActorFactory).GetRequiredMethod(
      name: nameof(IPolymorphicActorFactory.CreateAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string), typeof(int), typeof(CancellationToken) });

    MethodInfo factoryGetMethod = typeof(IPolymorphicActorFactory).GetRequiredMethod(
      name: nameof(IPolymorphicActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    MethodInfo interfaceBaseMethod = typeof(IPolymorphicActor).GetRequiredMethod(
      name: nameof(IPolymorphicActor.BaseMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementationBaseMethod = typeof(PolymorphicActor).GetRequiredMethod(
      name: nameof(PolymorphicActor.BaseMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementation1BaseMethod = typeof(PolymorphicActorImplementation1).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation1.BaseMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementation2BaseMethod = typeof(PolymorphicActorImplementation2).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation2.BaseMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo interfaceVirtualMethod = typeof(IPolymorphicActor).GetRequiredMethod(
      name: nameof(IPolymorphicActor.VirtualMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementationVirtualMethod = typeof(PolymorphicActor).GetRequiredMethod(
      name: nameof(PolymorphicActor.VirtualMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementation1VirtualMethod = typeof(PolymorphicActorImplementation1).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation1.VirtualMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementation2VirtualMethod = typeof(PolymorphicActorImplementation2).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation2.VirtualMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo interfaceAbstractMethod = typeof(IPolymorphicActor).GetRequiredMethod(
      name: nameof(IPolymorphicActor.AbstractMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementationAbstractMethod = typeof(PolymorphicActor).GetRequiredMethod(
      name: nameof(PolymorphicActor.AbstractMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementation1AbstractMethod = typeof(PolymorphicActorImplementation1).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation1.AbstractMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementation2AbstractMethod = typeof(PolymorphicActorImplementation2).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation2.AbstractMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo implementation1ActivityMethod = typeof(PolymorphicActorImplementation1).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation1.Activity),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    ConstructorInfo baseConstructor = typeof(PolymorphicActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor>) });

    ConstructorInfo implementation1Constructor = typeof(PolymorphicActorImplementation1).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor>) });

    ConstructorInfo implementation2Constructor = typeof(PolymorphicActorImplementation2).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor>), typeof(IService2) });

    ParameterInfo[] baseConstructorParameters = baseConstructor.GetParameters();
    ParameterInfo[] implementation1ConstructorParameters = implementation1Constructor.GetParameters();
    ParameterInfo[] implementation2ConstructorParameters = implementation2Constructor.GetParameters();

    FieldInfo stateField = typeof(PolymorphicActor).GetRequiredField(
      name: "_state",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] stateFields = typeof(PolymorphicActorState).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);


    IStateDependencyDescriptor baseStateDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(baseConstructorParameters[0])
      .SetName("state")
      .SetType(typeof(int))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(stateField));

    IServiceDependencyDescriptor baseService1Dependency = ServiceDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(baseConstructorParameters[1])
      .SetName("service1")
      .SetType(typeof(IService1))
      .SetIndex(1)
      .SetIsOptional(false));

    IContextDependencyDescriptor baseContextDependency = ContextDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(baseConstructorParameters[2])
      .SetName("context")
      .SetType(typeof(IActorContext<PolymorphicActor>))
      .SetImplementationType(typeof(PolymorphicActor))
      .SetIndex(2));

    IStateDependencyDescriptor implementation1StateDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(implementation1ConstructorParameters[0])
      .SetName("state")
      .SetType(typeof(int))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(stateField));

    IServiceDependencyDescriptor implementation1Service1Dependency = ServiceDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(implementation1ConstructorParameters[1])
      .SetName("service1")
      .SetType(typeof(IService1))
      .SetIndex(1)
      .SetIsOptional(false));

    IContextDependencyDescriptor implementation1ContextDependency = ContextDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(implementation1ConstructorParameters[2])
      .SetName("context")
      .SetType(typeof(IActorContext<PolymorphicActor>))
      .SetImplementationType(typeof(PolymorphicActorImplementation1))
      .SetIndex(2));

    IStateDependencyDescriptor implementation2StateDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(implementation2ConstructorParameters[0])
      .SetName("state")
      .SetType(typeof(int))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(stateField));

    IServiceDependencyDescriptor implementation2Service1Dependency = ServiceDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(implementation2ConstructorParameters[1])
      .SetName("service1")
      .SetType(typeof(IService1))
      .SetIndex(1)
      .SetIsOptional(false));

    IContextDependencyDescriptor implementation2ContextDependency = ContextDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(implementation2ConstructorParameters[2])
      .SetName("context")
      .SetType(typeof(IActorContext<PolymorphicActor>))
      .SetImplementationType(typeof(PolymorphicActor))
      .SetIndex(2));

    IServiceDependencyDescriptor implementation2Service2Dependency = ServiceDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(implementation2ConstructorParameters[3])
      .SetName("service2")
      .SetType(typeof(IService2))
      .SetIndex(3)
      .SetIsOptional(false));

    ITaskMethodDescriptor baseImplementationBaseMethod = TaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(1)
      .SetName(nameof(IPolymorphicActor.BaseMethod))
      .SetInterfaceMethod(interfaceBaseMethod)
      .SetImplementationMethod(implementationBaseMethod)
      .SetParameterTypes(Type.EmptyTypes)
      .SetActivityType(typeof(PolymorphicActorActivity1))
      .SetActivityFields()
      .SetHasCancellationTokenParameter(false)
      .SetCancellationTokenParameterPosition(-1));

    ITaskMethodDescriptor baseImplementationVirtualMethod = TaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(2)
      .SetName(nameof(IPolymorphicActor.VirtualMethod))
      .SetInterfaceMethod(interfaceVirtualMethod)
      .SetImplementationMethod(implementationVirtualMethod)
      .SetParameterTypes(Type.EmptyTypes)
      .SetActivityType(typeof(PolymorphicActorActivity2))
      .SetActivityFields()
      .SetHasCancellationTokenParameter(false)
      .SetCancellationTokenParameterPosition(-1));

    ITaskMethodDescriptor baseImplementationAbstractMethod = TaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(3)
      .SetInterfaceMethod(interfaceAbstractMethod)
      .SetName(nameof(IPolymorphicActor.AbstractMethod))
      .SetImplementationMethod(implementationAbstractMethod)
      .SetParameterTypes(Type.EmptyTypes)
      .SetActivityType(typeof(PolymorphicActorActivity3))
      .SetActivityFields()
      .SetHasCancellationTokenParameter(false)
      .SetCancellationTokenParameterPosition(-1));

    IImplementationDescriptor baseImplementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(PolymorphicActor))
      .SetConstructor(_ => _
        .SetConstructor(baseConstructor)
        .SetDependencies(baseStateDependency, baseService1Dependency, baseContextDependency)
        .SetContextDependencies(baseContextDependency)
        .SetServiceDependencies(baseService1Dependency)
        .SetStateDependencies(baseStateDependency))
      .SetMethods(baseImplementationBaseMethod, baseImplementationVirtualMethod, baseImplementationAbstractMethod));

    IImplementationDescriptor implementation1 = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(1)
      .SetType(typeof(PolymorphicActorImplementation1))
      .SetConstructor(_ => _
        .SetConstructor(implementation1Constructor)
        .SetDependencies(implementation1StateDependency, implementation1Service1Dependency, implementation1ContextDependency)
        .SetContextDependencies(implementation1ContextDependency)
        .SetServiceDependencies(implementation1Service1Dependency)
        .SetStateDependencies(implementation1StateDependency))
      .SetMethods(_ => _
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(1)
          .SetName(nameof(IPolymorphicActor.BaseMethod))
          .SetInterfaceMethod(interfaceBaseMethod)
          .SetImplementationMethod(implementation1BaseMethod)
          .SetParameterTypes(Type.EmptyTypes)
          .SetActivityType(typeof(PolymorphicActorActivity1))
          .SetActivityFields()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(2)
          .SetName(nameof(IPolymorphicActor.VirtualMethod))
          .SetInterfaceMethod(interfaceVirtualMethod)
          .SetImplementationMethod(implementation1VirtualMethod)
          .SetParameterTypes(Type.EmptyTypes)
          .SetActivityType(typeof(PolymorphicActorActivity2))
          .SetActivityFields()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(3)
          .SetName(nameof(IPolymorphicActor.AbstractMethod))
          .SetInterfaceMethod(interfaceAbstractMethod)
          .SetImplementationMethod(implementation1AbstractMethod)
          .SetParameterTypes(Type.EmptyTypes)
          .SetActivityType(typeof(PolymorphicActorActivity3))
          .SetActivityFields()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))));

    IImplementationDescriptor implementation2 = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(2)
      .SetType(typeof(PolymorphicActorImplementation2))
      .SetConstructor(_ => _
        .SetConstructor(implementation2Constructor)
        .SetDependencies(implementation2StateDependency, implementation2Service1Dependency, implementation2ContextDependency, implementation2Service2Dependency)
        .SetContextDependencies(implementation2ContextDependency)
        .SetServiceDependencies(implementation2Service1Dependency, implementation2Service2Dependency)
        .SetStateDependencies(implementation2StateDependency))
      .SetMethods(_ => _
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(1)
          .SetName(nameof(IPolymorphicActor.BaseMethod))
          .SetInterfaceMethod(interfaceBaseMethod)
          .SetImplementationMethod(implementation2BaseMethod)
          .SetParameterTypes(Type.EmptyTypes)
          .SetActivityType(typeof(PolymorphicActorActivity1))
          .SetActivityFields()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(2)
          .SetName(nameof(IPolymorphicActor.VirtualMethod))
          .SetInterfaceMethod(interfaceVirtualMethod)
          .SetImplementationMethod(implementation2VirtualMethod)
          .SetParameterTypes(Type.EmptyTypes)
          .SetActivityType(typeof(PolymorphicActorActivity2))
          .SetActivityFields()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(3)
          .SetName(nameof(IPolymorphicActor.AbstractMethod))
          .SetInterfaceMethod(interfaceAbstractMethod)
          .SetImplementationMethod(implementation2AbstractMethod)
          .SetParameterTypes(Type.EmptyTypes)
          .SetActivityType(typeof(PolymorphicActorActivity3))
          .SetActivityFields()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IPolymorphicActor))
      .SetActorType(typeof(PolymorphicActor))
      .SetBaseImplementation(baseImplementation)
      .SetDefaultImplementation(implementation2)
      .SetImplementations(implementation1, implementation2)
      .SetIsPolymorphic(true)
      .SetIsVirtual(false)
      .SetActivities(_ => _
        .Add(baseImplementationBaseMethod)
        .Add(baseImplementationVirtualMethod)
        .Add(baseImplementationAbstractMethod)
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(4)
          .SetName(nameof(PolymorphicActorImplementation1.Activity))
          .SetImplementationMethod(implementation1ActivityMethod)
          .SetParameterTypes(Type.EmptyTypes)
          .SetActivityType(typeof(PolymorphicActorActivity4))
          .SetActivityFields()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1)))
      .SetId(_ => _
        .SetType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetIdProperty(null))
      .SetState(_ => _
        .SetType(typeof(PolymorphicActorState))
        .SetFields(stateFields)
        .SetDefaultValue(null))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IPolymorphicActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));
  }
}
