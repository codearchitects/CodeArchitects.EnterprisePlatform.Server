using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures.Examples;

internal class StandardActorStateComponent
{
}

internal interface IStandardActor
{
  Task TaskMethod(int arg);

  Task<int> TaskTMethod(int arg, CancellationToken cancellationToken);
  
  ValueTask ValueTaskMethod(CancellationToken cancellationToken);
  
  /* [Stateless] */
  ValueTask<string> ValueTaskTMethod();
}

internal class StandardActor : IStandardActor
{
  /* [State] */
  private readonly string _state1;
  private readonly IService1 _service1;
  /* [State] */
  private readonly StandardActorStateComponent _state2;
  private readonly IActorContext<StandardActor> _context;
  private readonly IService2 _service2;

  /* [ActorConstructor] */
  public StandardActor(string state1, IService1 service1, StandardActorStateComponent state2, IActorContext<StandardActor> context, IService2 service2)
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
}

internal interface IStandardActorFactory
{
  Task<IStandardActor> CreateAsync(string id, string state1, StandardActorStateComponent state2, CancellationToken cancellationToken = default);
  IStandardActor Get(string id);
}

internal class StandardActorState
{
  public int State1 { get; set; }
  public StandardActorStateComponent State2 { get; set; } = default!;
}

internal static class StandardActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  static StandardActorFixture()
  {
    FieldInfo state1Field = typeof(StandardActor).GetRequiredField(
      name: "_state1",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo state2Field = typeof(StandardActor).GetRequiredField(
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

    ConstructorInfo constructor = typeof(StandardActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string), typeof(IService1), typeof(StandardActorStateComponent), typeof(IActorContext<StandardActor>), typeof(IService2) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

    MethodInfo factoryCreateAsyncMethod = typeof(IStandardActorFactory).GetRequiredMethod(
      name: nameof(IStandardActorFactory.CreateAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string), typeof(string), typeof(StandardActorStateComponent) });

    MethodInfo factoryGetMethod = typeof(IStandardActorFactory).GetRequiredMethod(
      name: nameof(IStandardActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    IStateDependencyDescriptor state1Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("state1")
      .SetType(typeof(string))
      .SetIndex(0)
      .SetCategoryIndex(0)
      .SetField(state1Field)
      .SetDefault(Optional<object?>.None));

    IServiceDependencyDescriptor service1Dependency = ServiceDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[1])
      .SetName("service1")
      .SetType(typeof(IService1))
      .SetIndex(1)
      .SetCategoryIndex(0));

    IStateDependencyDescriptor state2Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[2])
      .SetName("state2")
      .SetType(typeof(StandardActorStateComponent))
      .SetIndex(2)
      .SetCategoryIndex(1)
      .SetField(state2Field)
      .SetDefault(new StandardActorStateComponent()));

    IContextDependencyDescriptor contextDependency = ContextDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[3])
      .SetName("context")
      .SetType(typeof(IActorContext<StandardActor>))
      .SetIndex(3)
      .SetCategoryIndex(0));

    IServiceDependencyDescriptor service2Dependency = ServiceDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[1])
      .SetName("service2")
      .SetType(typeof(IService2))
      .SetIndex(4)
      .SetCategoryIndex(1));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(StandardActor))
      .SetConstructor(_ => _
        .SetConstructor(constructor)
        .SetDependencies(state1Dependency, service1Dependency, state2Dependency, contextDependency, service2Dependency)
        .SetContextDependency(contextDependency)
        .SetServiceDependencies(service1Dependency, service2Dependency)
        .SetStateDependencies(state1Dependency, state2Dependency)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStandardActor))
      .SetImplementationType(typeof(StandardActor))
      .SetIsVirtual(false)
      .SetStartingImplementation(implementation)
      .SetImplementations(implementation)
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetStateType(typeof(StandardActorState))
        .SetIsStateless(false)
        .SetDefaultComponents(null as IReadOnlyList<object>))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IStandardActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod))
      .SetMethods(_ => _
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceTaskMethod)
          .SetImplementationMethod(implementationTaskMethod)
          .SetParameterTypes(typeof(int))
          .SetIsStateless(false)
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskTMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceTaskTMethod)
          .SetImplementationMethod(implementationTaskTMethod)
          .SetParameterTypes(typeof(int), typeof(CancellationToken))
          .SetIsStateless(false)
          .SetHasCancellationTokenParameter(true)
          .SetCancellationTokenParameterPosition(1)
          .SetResultType(typeof(int)))
        .Add<ValueTaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceValueTaskMethod)
          .SetImplementationMethod(implementationValueTaskMethod)
          .SetParameterTypes(typeof(CancellationToken))
          .SetIsStateless(false)
          .SetHasCancellationTokenParameter(true)
          .SetCancellationTokenParameterPosition(0))
        .Add<ValueTaskTMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceValueTaskTMethod)
          .SetImplementationMethod(implementationValueTaskTMethod)
          .SetParameterTypes()
          .SetIsStateless(true)
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1)
          .SetResultType(typeof(int)))));
  }
}
