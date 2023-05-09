using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

internal interface IPolymorphicActor
{
  Task BaseMethod();

  Task VirtualMethod();

  Task AbstractMethod();
}

[Actor]
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

[ActorImplementation<PolymorphicActor>]
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

[ActorImplementation(typeof(PolymorphicActor), IsDefault = true)]
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

internal class PolymorphicActorState : Infrastructure.PolymorphicActorState
{
  public int _0 { get; set; }
}

[ActorFactory<PolymorphicActor>]
internal interface IPolymorphicActorFactory
{
  Task<IPolymorphicActor> CreateAsync(string id, int state, CancellationToken cancellationToken = default);
  IPolymorphicActor Get(string id);
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


    ITaskMethodDescriptor baseImplementationBaseMethod = TaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(1)
      .SetName(nameof(IPolymorphicActor.BaseMethod))
      .SetInterfaceMethod(interfaceBaseMethod)
      .SetImplementationMethod(implementationBaseMethod)
      .SetParameterTypes(Type.EmptyTypes)
      .SetActivityType(typeof(PolymorphicActorActivity1))
      .SetActivityFields()
      .SetHasCancellationTokenParameter(false));

    ITaskMethodDescriptor baseImplementationVirtualMethod = TaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(2)
      .SetName(nameof(IPolymorphicActor.VirtualMethod))
      .SetInterfaceMethod(interfaceVirtualMethod)
      .SetImplementationMethod(implementationVirtualMethod)
      .SetParameterTypes(Type.EmptyTypes)
      .SetActivityType(typeof(PolymorphicActorActivity2))
      .SetActivityFields()
      .SetHasCancellationTokenParameter(false));

    ITaskMethodDescriptor baseImplementationAbstractMethod = TaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(3)
      .SetName(nameof(IPolymorphicActor.AbstractMethod))
      .SetInterfaceMethod(interfaceAbstractMethod)
      .SetImplementationMethod(implementationAbstractMethod)
      .SetParameterTypes(Type.EmptyTypes)
      .SetActivityType(typeof(PolymorphicActorActivity3))
      .SetActivityFields()
      .SetHasCancellationTokenParameter(false));

    IImplementationDescriptor baseImplementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(PolymorphicActor)));

    IImplementationDescriptor implementation1 = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(1)
      .SetType(typeof(PolymorphicActorImplementation1)));

    IImplementationDescriptor implementation2 = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(2)
      .SetType(typeof(PolymorphicActorImplementation2)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IPolymorphicActor))
      .SetActorType(typeof(PolymorphicActor))
      .SetBaseImplementation(baseImplementation)
      .SetDefaultImplementation(implementation2)
      .SetImplementations(implementation1, implementation2)
      .SetIsPolymorphic(true)
      .SetIsVirtual(false)
      .SetActivityBaseType(typeof(PolymorphicActorActivity))
      .SetMethods(baseImplementationBaseMethod, baseImplementationVirtualMethod, baseImplementationAbstractMethod)
      .SetActivities(_ => _
        .Add(baseImplementationBaseMethod)
        .Add(baseImplementationVirtualMethod)
        .Add(baseImplementationAbstractMethod)
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(4)
          .SetName(nameof(PolymorphicActorImplementation1.Activity))
          .SetInterfaceMethod(null)
          .SetImplementationMethod(implementation1ActivityMethod)
          .SetParameterTypes(Type.EmptyTypes)
          .SetActivityType(typeof(PolymorphicActorActivity4))
          .SetActivityFields()
          .SetHasCancellationTokenParameter(false)))
      .SetId(_ => _
        .SetType(typeof(string))
        .SetHasIdSource(false)
        .SetStateIndex(-1))
      .SetState(_ => _
        .SetType(new StateTypeDelegator(typeof(PolymorphicActorState)))
        .SetFields(stateFields)
        .SetDefaultValue(null))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IPolymorphicActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod))
      .SetMessageHandlers());
  }

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(PolymorphicActor);
    Type activityBaseType = typeof(PolymorphicActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), true))
      .Returns(Descriptor.State.Type);

    activityTypeBuilderMock
      .Setup(x => x.BuildBase(actorType))
      .Returns(activityBaseType);

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 1), actorType, activityBaseType))
      .Returns(typeof(PolymorphicActorActivity1));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 2), actorType, activityBaseType))
      .Returns(typeof(PolymorphicActorActivity2));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 3), actorType, activityBaseType))
      .Returns(typeof(PolymorphicActorActivity3));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 4), actorType, activityBaseType))
      .Returns(typeof(PolymorphicActorActivity4));
  }

  public static void AssertValidDescriptor(IActorDescriptor descriptor)
  {
    descriptor.Should().BeAssignableTo<IActorDescriptor<PolymorphicActor, PolymorphicActorState>>();
    descriptor.Should().BeEquivalentTo(Descriptor, opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
    descriptor.GetImplementation(typeof(PolymorphicActorImplementation1)).Should().BeEquivalentTo(Descriptor.Implementations.ElementAt(0), opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
    descriptor.GetImplementation(typeof(PolymorphicActorImplementation2)).Should().BeEquivalentTo(Descriptor.Implementations.ElementAt(1), opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
  }
}
