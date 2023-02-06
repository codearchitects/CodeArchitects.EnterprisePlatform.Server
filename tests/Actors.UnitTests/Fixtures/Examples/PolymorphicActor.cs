using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures.Examples;

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
  {
    throw new NotImplementedException();
  }

  public virtual Task VirtualMethod()
  {
    throw new NotImplementedException();
  }

  public abstract Task AbstractMethod();
}

internal class PolymorphicActorImplementation1 : PolymorphicActor
{
  [ActorConstructor]
  public PolymorphicActorImplementation1(int state, IService1 service1, IActorContext<PolymorphicActor, PolymorphicActorImplementation1> context)
    : base(state, service1, context)
  {
  }

  public PolymorphicActorImplementation1()
  {
  }

  public override Task AbstractMethod()
  {
    throw new NotImplementedException();
  }

  public override Task VirtualMethod()
  {
    throw new NotImplementedException();
  }
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
  {
    throw new NotImplementedException();
  }
}

[ActorFactory<PolymorphicActor>]
internal interface IPolymorphicActorFactory
{
  Task<IPolymorphicActor> CreateAsync(string id, int state, CancellationToken cancellationToken = default);
  IPolymorphicActor Get(string id);
}

internal class PolymorphicActorState
{
  public int _state { get; set; }
  public int _discriminator { get; set; } = default!;
}

internal static class PolymorphicActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  private static readonly ConstructorInfo s_baseConstructor;
  private static readonly ConstructorInfo s_implementation1Constructor;
  private static readonly ConstructorInfo s_implementation2Constructor;
  private static readonly FieldInfo s_stateField;
  private static readonly MethodInfo s_implementationBaseMethod;
  private static readonly MethodInfo s_implementationVirtualMethod;
  private static readonly MethodInfo s_implementationAbstractMethod;
  private static readonly MethodInfo s_implementation1BaseMethod;
  private static readonly MethodInfo s_implementation1VirtualMethod;
  private static readonly MethodInfo s_implementation1AbstractMethod;
  private static readonly MethodInfo s_implementation2BaseMethod;
  private static readonly MethodInfo s_implementation2VirtualMethod;
  private static readonly MethodInfo s_implementation2AbstractMethod;

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

    s_implementationBaseMethod = typeof(PolymorphicActor).GetRequiredMethod(
      name: nameof(PolymorphicActor.BaseMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementation1BaseMethod = typeof(PolymorphicActorImplementation1).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation1.BaseMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementation2BaseMethod = typeof(PolymorphicActorImplementation2).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation2.BaseMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo interfaceVirtualMethod = typeof(IPolymorphicActor).GetRequiredMethod(
      name: nameof(IPolymorphicActor.VirtualMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementationVirtualMethod = typeof(PolymorphicActor).GetRequiredMethod(
      name: nameof(PolymorphicActor.VirtualMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementation1VirtualMethod = typeof(PolymorphicActorImplementation1).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation1.VirtualMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementation2VirtualMethod = typeof(PolymorphicActorImplementation2).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation2.VirtualMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo interfaceAbstractMethod = typeof(IPolymorphicActor).GetRequiredMethod(
      name: nameof(IPolymorphicActor.AbstractMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementationAbstractMethod = typeof(PolymorphicActor).GetRequiredMethod(
      name: nameof(PolymorphicActor.AbstractMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementation1AbstractMethod = typeof(PolymorphicActorImplementation1).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation1.AbstractMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementation2AbstractMethod = typeof(PolymorphicActorImplementation2).GetRequiredMethod(
      name: nameof(PolymorphicActorImplementation2.AbstractMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_baseConstructor = typeof(PolymorphicActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor>) });

    s_implementation1Constructor = typeof(PolymorphicActorImplementation1).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor, PolymorphicActorImplementation1>) });

    s_implementation2Constructor = typeof(PolymorphicActorImplementation2).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor>), typeof(IService2) });

    ParameterInfo[] baseConstructorParameters = s_baseConstructor.GetParameters();
    ParameterInfo[] implementation1ConstructorParameters = s_implementation1Constructor.GetParameters();
    ParameterInfo[] implementation2ConstructorParameters = s_implementation2Constructor.GetParameters();

    s_stateField = typeof(PolymorphicActor).GetRequiredField(
      name: "_state",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo discriminatorField = typeof(PolymorphicActorState).GetRequiredField(
      name: $"<{nameof(PolymorphicActorState._discriminator)}>k__BackingField",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    IStateDependencyDescriptor baseStateDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(baseConstructorParameters[0])
      .SetName("state")
      .SetType(typeof(int))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(s_stateField));

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
      .SetField(s_stateField));

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
      .SetType(typeof(IActorContext<PolymorphicActor, PolymorphicActorImplementation1>))
      .SetImplementationType(typeof(PolymorphicActorImplementation1))
      .SetIndex(2));

    IStateDependencyDescriptor implementation2StateDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(implementation2ConstructorParameters[0])
      .SetName("state")
      .SetType(typeof(int))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(s_stateField));

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

    IImplementationDescriptor baseImplementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(PolymorphicActor))
      .SetConstructor(_ => _
        .SetConstructor(s_baseConstructor)
        .SetDependencies(baseStateDependency, baseService1Dependency, baseContextDependency)
        .SetContextDependencies(baseContextDependency)
        .SetServiceDependencies(baseService1Dependency)
        .SetStateDependencies(baseStateDependency))
      .SetMethods(_ => _
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceBaseMethod)
          .SetImplementationMethod(s_implementationBaseMethod)
          .SetParameterTypes()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceVirtualMethod)
          .SetImplementationMethod(s_implementationVirtualMethod)
          .SetParameterTypes()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceAbstractMethod)
          .SetImplementationMethod(s_implementationAbstractMethod)
          .SetParameterTypes()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))));

    IImplementationDescriptor implementation1 = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(PolymorphicActorImplementation1))
      .SetConstructor(_ => _
        .SetConstructor(s_implementation1Constructor)
        .SetDependencies(implementation1StateDependency, implementation1Service1Dependency, implementation1ContextDependency)
        .SetContextDependencies(implementation1ContextDependency)
        .SetServiceDependencies(implementation1Service1Dependency)
        .SetStateDependencies(implementation1StateDependency))
      .SetMethods(_ => _
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceBaseMethod)
          .SetImplementationMethod(s_implementation1BaseMethod)
          .SetParameterTypes()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceVirtualMethod)
          .SetImplementationMethod(s_implementation1VirtualMethod)
          .SetParameterTypes()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceAbstractMethod)
          .SetImplementationMethod(s_implementation1AbstractMethod)
          .SetParameterTypes()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))));

    IImplementationDescriptor implementation2 = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(PolymorphicActorImplementation2))
      .SetConstructor(_ => _
        .SetConstructor(s_implementation2Constructor)
        .SetDependencies(implementation2StateDependency, implementation2Service1Dependency, implementation2ContextDependency, implementation2Service2Dependency)
        .SetContextDependencies(implementation2ContextDependency)
        .SetServiceDependencies(implementation2Service1Dependency, implementation2Service2Dependency)
        .SetStateDependencies(implementation2StateDependency))
      .SetMethods(_ => _
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceBaseMethod)
          .SetImplementationMethod(s_implementation2BaseMethod)
          .SetParameterTypes()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceVirtualMethod)
          .SetImplementationMethod(s_implementation2VirtualMethod)
          .SetParameterTypes()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceAbstractMethod)
          .SetImplementationMethod(s_implementation2AbstractMethod)
          .SetParameterTypes()
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IPolymorphicActor))
      .SetActorType(typeof(PolymorphicActor))
      .SetBaseImplementation(baseImplementation)
      .SetDefaultImplementation(implementation2)
      .SetImplementations(implementation1, implementation2)
      .SetIsPolymorphic(true)
      .SetIsStateless(false)
      .SetIsVirtual(false)
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetType(new StateTypeDelegator(typeof(PolymorphicActorState), nameof(PolymorphicActorState._discriminator)))
        .SetStateFields(s_stateField)
        .SetDiscriminatorField(discriminatorField)
        .SetDefaultValue(null))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IPolymorphicActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));
  }
}
