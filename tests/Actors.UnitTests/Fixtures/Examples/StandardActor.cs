using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Common;
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
  void VoidMethod();

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

  public void VoidMethod()
    => throw new NotImplementedException();

  public Task TaskMethod(int arg)
    => throw new NotImplementedException();

  public Task<int> TaskTMethod(int arg, CancellationToken cancellationToken)
    => throw new NotImplementedException();

  public ValueTask ValueTaskMethod(CancellationToken cancellationToken)
    => throw new NotImplementedException();

  [Stateless]
  public ValueTask<string> ValueTaskTMethod()
    => throw new NotImplementedException();

  public void Dispose()
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

internal static class StandardActorFixture
{
  public static readonly IActorDescriptor Descriptor;
  public static readonly IActorMetadata Metadata;

  private static readonly ConstructorInfo s_constructor;
  private static readonly FieldInfo s_state1Field;
  private static readonly FieldInfo s_state2Field;
  private static readonly MethodInfo s_implementationVoidMethod;
  private static readonly MethodInfo s_implementationTaskMethod;
  private static readonly MethodInfo s_implementationTaskTMethod;
  private static readonly MethodInfo s_implementationValueTaskMethod;
  private static readonly MethodInfo s_implementationValueTaskTMethod;

  static StandardActorFixture()
  {
    s_state1Field = typeof(StandardActor).GetRequiredField(
      name: "_state1",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    s_state2Field = typeof(StandardActor).GetRequiredField(
      name: "_state2",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    MethodInfo interfaceVoidMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.VoidMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementationVoidMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.VoidMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    MethodInfo interfaceTaskMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.TaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    s_implementationTaskMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.TaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    MethodInfo interfaceTaskTMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.TaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(CancellationToken) });

    s_implementationTaskTMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.TaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(CancellationToken) });

    MethodInfo interfaceValueTaskMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.ValueTaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(CancellationToken) });

    s_implementationValueTaskMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.ValueTaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(CancellationToken) });

    MethodInfo interfaceValueTaskTMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.ValueTaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_implementationValueTaskTMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.ValueTaskTMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

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

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(StandardActor))
      .SetConstructor(_ => _
        .SetConstructor(s_constructor)
        .SetDependencies(state1Dependency, service1Dependency, state2Dependency, contextDependency, service2Dependency)
        .SetContextDependency(contextDependency)
        .SetServiceDependencies(service1Dependency, service2Dependency)
        .SetStateDependencies(state1Dependency, state2Dependency))
      .SetMethods(_ => _
        .Add<VoidMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceVoidMethod)
          .SetImplementationMethod(s_implementationVoidMethod)
          .SetParameterTypes()
          .SetIsStateless(false)
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceTaskMethod)
          .SetImplementationMethod(s_implementationTaskMethod)
          .SetParameterTypes(typeof(int))
          .SetIsStateless(false)
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1))
        .Add<TaskTMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceTaskTMethod)
          .SetImplementationMethod(s_implementationTaskTMethod)
          .SetParameterTypes(typeof(int), typeof(CancellationToken))
          .SetIsStateless(false)
          .SetHasCancellationTokenParameter(true)
          .SetCancellationTokenParameterPosition(1)
          .SetResultType(typeof(int)))
        .Add<ValueTaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceValueTaskMethod)
          .SetImplementationMethod(s_implementationValueTaskMethod)
          .SetParameterTypes(typeof(CancellationToken))
          .SetIsStateless(false)
          .SetHasCancellationTokenParameter(true)
          .SetCancellationTokenParameterPosition(0))
        .Add<ValueTaskTMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetInterfaceMethod(interfaceValueTaskTMethod)
          .SetImplementationMethod(s_implementationValueTaskTMethod)
          .SetParameterTypes()
          .SetIsStateless(true)
          .SetHasCancellationTokenParameter(false)
          .SetCancellationTokenParameterPosition(-1)
          .SetResultType(typeof(string)))));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStandardActor))
      .SetActorType(typeof(StandardActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetStateType(typeof(StandardActorState))
        .SetIsStateless(false)
        .SetIsVirtual(false)
        .SetFields(s_state1Field, s_state2Field)
        .SetDiscriminatorField(null)
        .SetDefaultValues(null as IReadOnlyList<object>))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IStandardActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));


    IImplementationMetadata baseImplementationMetadata = ImplementationMetadataBuilder.Build(_ => _
      .SetIsDefault(false)
      .SetImplementationType(typeof(StandardActor))
      .SetConstructor(s_constructor)
      .SetHasStateFields(true)
      .Setup(mock => mock
        .Setup(x => x.GetMethodMetadata(s_implementationVoidMethod).IsStateless)
        .Returns(false))
      .Setup(mock => mock
        .Setup(x => x.GetMethodMetadata(s_implementationTaskMethod).IsStateless)
        .Returns(false))
      .Setup(mock => mock
        .Setup(x => x.GetMethodMetadata(s_implementationTaskTMethod).IsStateless)
        .Returns(false))
      .Setup(mock => mock
        .Setup(x => x.GetMethodMetadata(s_implementationValueTaskMethod).IsStateless)
        .Returns(false))
      .Setup(mock => mock
        .Setup(x => x.GetMethodMetadata(s_implementationValueTaskTMethod).IsStateless)
        .Returns(true)));

    Metadata = ActorMetadataBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStandardActor))
      .SetActorType(typeof(StandardActor))
      .SetIsExplicitVirtual(false)
      .SetFactoryType(typeof(IStandardActorFactory))
      .SetStateFields(_ => _
        .Add(_ => _
          .SetField(s_state1Field)
          .SetDefaultValue(Optional<object?>.None)
          .Setup(mock => mock
            .Setup(x => x.IsActorIdSource(out It.Ref<PropertyInfo?>.IsAny))
            .Returns(false)))
        .Add(_ => _
          .SetField(s_state2Field)
          .SetDefaultValue(Optional<object?>.None)
          .Setup(mock => mock
            .Setup(x => x.IsActorIdSource(out It.Ref<PropertyInfo?>.IsAny))
            .Returns(false))))
      .SetBaseImplementation(baseImplementationMetadata)
      .SetImplementations());
  }

  public static void AssertValidMetadata(IActorMetadata metadata)
  {
    PropertyInfo? actorIdProperty;

    metadata.InterfaceType.Should().Be<IStandardActor>();
    metadata.ActorType.Should().Be<StandardActor>();
    metadata.IsExplicitVirtual.Should().BeFalse();
    metadata.FactoryType.Should().Be<IStandardActorFactory>();
    metadata.StateFields.Should().HaveCount(2);

    IStateFieldMetadata state1Field = metadata.StateFields.ElementAt(0);
    state1Field.Field.Should().Be(s_state1Field);
    state1Field.DefaultValue.Should().Be(Optional<object?>.None);
    state1Field.IsActorIdSource(out actorIdProperty).Should().BeFalse();
    actorIdProperty.Should().BeNull();

    IStateFieldMetadata state2Field = metadata.StateFields.ElementAt(1);
    state2Field.Field.Should().Be(s_state2Field);
    state2Field.DefaultValue.Should().Be(Optional<object?>.None);
    state2Field.IsActorIdSource(out actorIdProperty).Should().BeFalse();
    actorIdProperty.Should().BeNull();

    metadata.BaseImplementation.IsDefault.Should().BeFalse();
    metadata.BaseImplementation.ImplementationType.Should().Be<StandardActor>();
    metadata.BaseImplementation.Constructor.Should().BeSameAs(s_constructor);
    metadata.BaseImplementation.HasStateFields.Should().BeTrue();

    IMethodMetadata voidMethodMetadata = metadata.BaseImplementation.GetMethodMetadata(s_implementationVoidMethod);
    voidMethodMetadata.IsStateless.Should().BeFalse();

    IMethodMetadata taskMethodMetadata = metadata.BaseImplementation.GetMethodMetadata(s_implementationTaskMethod);
    taskMethodMetadata.IsStateless.Should().BeFalse();

    IMethodMetadata taskTMethodMetadata = metadata.BaseImplementation.GetMethodMetadata(s_implementationTaskTMethod);
    taskTMethodMetadata.IsStateless.Should().BeFalse();

    IMethodMetadata valueTaskMethodMetadata = metadata.BaseImplementation.GetMethodMetadata(s_implementationValueTaskMethod);
    valueTaskMethodMetadata.IsStateless.Should().BeFalse();

    IMethodMetadata valueTaskTMethodMetadata = metadata.BaseImplementation.GetMethodMetadata(s_implementationValueTaskTMethod);
    valueTaskTMethodMetadata.IsStateless.Should().BeTrue();

    metadata.Implementations.Should().BeEmpty();
  }
}
