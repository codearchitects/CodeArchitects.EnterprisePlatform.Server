using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Messaging;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Actors.Metadata.Implementation;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Messaging;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace CodeArchitects.Platform.Actors.TestModel;

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

  Task<int> TaskMethod(int arg, CancellationToken cancellationToken); // Same name as previous method to test overloads
  
  ValueTask ValueTaskMethod(CancellationToken cancellationToken);
  
  ValueTask<string> ValueTaskTMethod();
}

internal interface IOtherInterface { }

internal class ActorMessage : IActorMessage<string>
{
  public string ActorId { get; set; } = default!;
}

[Actor<IStandardActor>]
internal class StandardActor : IStandardActor, IOtherInterface, IMessageHandler<ActorMessage>
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

  public virtual Task TaskMethod(int arg)
    => throw new NotImplementedException();

  public virtual Task<int> TaskMethod(int arg, CancellationToken cancellationToken)
    => throw new NotImplementedException();

  public virtual ValueTask ValueTaskMethod(CancellationToken cancellationToken)
    => throw new NotImplementedException();

  public virtual ValueTask<string> ValueTaskTMethod()
    => throw new NotImplementedException();

  public virtual void VoidActivity()
    => throw new NotImplementedException();

  public virtual Task ActivityOverload(int arg)
    => throw new NotImplementedException();

  public virtual Task ActivityOverload(string arg)
    => throw new NotImplementedException();

  [MessageHandler("bus", "topic")]
  public Task HandleAsync(ActorMessage message, CancellationToken cancellationToken)
    => throw new NotImplementedException();
}

internal class StandardActorState : OrdinaryActorState
{
  public string _0 { get; set; } = default!;
  public StandardActorStateComponent _1 { get; set; } = default!;

  public override bool Equals(object? obj) => obj is StandardActorState other && (_0, _1).Equals((other._0, other._1));

  public override int GetHashCode() => HashCode.Combine(_0, _1);
}

[ActorFactory<StandardActor>]
internal interface IStandardActorFactory
{
  Task<IStandardActor> CreateAsync(string id, string state1, StandardActorStateComponent state2, CancellationToken cancellationToken = default);
  IStandardActor Get(string id);
}

internal abstract class StandardActorActivity : Activity<StandardActor>
{
}

internal class StandardActorActivity1 : StandardActorActivity
{
  public override int Id => 1;

  public int arg { get; set; }

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => actor.TaskMethod(arg);
}

internal class StandardActorActivity2 : StandardActorActivity
{
  public override int Id => 2;

  public int arg { get; set; }

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => actor.TaskMethod(arg, cancellationToken);
}

internal class StandardActorActivity3 : StandardActorActivity
{
  public override int Id => 3;

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => actor.ValueTaskMethod(cancellationToken).AsTask();
}

internal class StandardActorActivity4 : StandardActorActivity
{
  public override int Id => 4;

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => actor.ValueTaskTMethod().AsTask();
}

internal class StandardActorActivity5 : StandardActorActivity
{
  public override int Id => 5;

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
  {
    actor.VoidActivity();
    return Task.CompletedTask;
  }
}

internal class StandardActorActivity6 : StandardActorActivity
{
  public override int Id => 6;

  public int arg { get; set; }

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => actor.ActivityOverload(arg);
}

internal class StandardActorActivity7 : StandardActorActivity
{
  public override int Id => 7;

  public string arg { get; set; } = default!;

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => actor.ActivityOverload(arg);
}

internal class StandardActorActivity8 : StandardActorActivity
{
  public override int Id => 8;

  public ActorMessage message { get; set; } = default!;

  public override Task ExecuteAsync(StandardActor actor, CancellationToken cancellationToken)
    => actor.HandleAsync(message, cancellationToken);
}

internal class StandardActorActivityTypeResolver : DefaultJsonTypeInfoResolver
{
  public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
  {
    JsonTypeInfo info = base.GetTypeInfo(type, options);

    if (info.Type == typeof(StandardActorActivity))
    {
      info.PolymorphismOptions = new JsonPolymorphismOptions
      {
        TypeDiscriminatorPropertyName = ":id",
        IgnoreUnrecognizedTypeDiscriminators = false,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        DerivedTypes =
        {
          new JsonDerivedType(typeof(StandardActorActivity1), 1),
          new JsonDerivedType(typeof(StandardActorActivity2), 2),
          new JsonDerivedType(typeof(StandardActorActivity3), 3),
          new JsonDerivedType(typeof(StandardActorActivity4), 4),
          new JsonDerivedType(typeof(StandardActorActivity5), 5),
          new JsonDerivedType(typeof(StandardActorActivity6), 6),
          new JsonDerivedType(typeof(StandardActorActivity7), 7),
          new JsonDerivedType(typeof(StandardActorActivity8), 8)
        }
      };
    }

    return info;
  }
}

internal static class StandardActorFixture
{
  public static readonly IActorDescriptor Descriptor;
  public static FieldInfo[] StateFields;

  static StandardActorFixture()
  {
    JsonSerializerOptions jsonSerializerOptions = new()
    {
      TypeInfoResolver = new StandardActorActivityTypeResolver(),
      IgnoreReadOnlyProperties = true
    };

    FieldInfo state1Field = typeof(StandardActor).GetRequiredField(
      name: "_state1",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo state2Field = typeof(StandardActor).GetRequiredField(
      name: "_state2",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    StateFields = new[] { state1Field, state2Field };

    MethodInfo interfaceTaskMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.TaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    MethodInfo implementationTaskMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.TaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    MethodInfo interfaceTaskTMethod = typeof(IStandardActor).GetRequiredMethod(
      name: nameof(IStandardActor.TaskMethod),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(CancellationToken) });

    MethodInfo implementationTaskTMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.TaskMethod),
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

    MethodInfo interfaceHandlerMethod = typeof(IMessageHandler<ActorMessage>).GetRequiredMethod(
      name: nameof(IMessageHandler<ActorMessage>.HandleAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(ActorMessage), typeof(CancellationToken) });

    MethodInfo implementationHandlerMethod = typeof(StandardActor).GetRequiredMethod(
      name: nameof(StandardActor.HandleAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(ActorMessage), typeof(CancellationToken) });

    ConstructorInfo constructor = typeof(StandardActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string), typeof(IService1), typeof(StandardActorStateComponent), typeof(IActorContext<StandardActor>), typeof(IService2) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

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

    FieldInfo[] activity8Fields = typeof(StandardActorActivity8).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    FieldInfo[] stateFields = typeof(StandardActorState).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    ITaskMethodDescriptor taskMethod = TaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(1)
      .SetName(nameof(IStandardActor.TaskMethod))
      .SetInterfaceMethod(interfaceTaskMethod)
      .SetImplementationMethod(implementationTaskMethod)
      .SetParameterTypes(new[] { typeof(int) })
      .SetActivityType(typeof(StandardActorActivity1))
      .SetActivityFields(activity1Fields)
      .SetHasCancellationTokenParameter(false));

    ITaskTMethodDescriptor taskTMethod = TaskTMethodDescriptorBuilder.Build(_ => _
      .InitDefaults(typeof(int))
      .SetId(2)
      .SetName(nameof(IStandardActor.TaskMethod))
      .SetInterfaceMethod(interfaceTaskTMethod)
      .SetImplementationMethod(implementationTaskTMethod)
      .SetParameterTypes(new[] { typeof(int), typeof(CancellationToken) })
      .SetActivityType(typeof(StandardActorActivity2))
      .SetActivityFields(activity2Fields)
      .SetHasCancellationTokenParameter(true));

    IValueTaskMethodDescriptor valueTaskMethod = ValueTaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(3)
      .SetName(nameof(IStandardActor.ValueTaskMethod))
      .SetInterfaceMethod(interfaceValueTaskMethod)
      .SetImplementationMethod(implementationValueTaskMethod)
      .SetParameterTypes(new[] { typeof(CancellationToken) })
      .SetActivityType(typeof(StandardActorActivity3))
      .SetActivityFields(activity3Fields)
      .SetHasCancellationTokenParameter(true));

    IValueTaskTMethodDescriptor valueTaskTMethod = ValueTaskTMethodDescriptorBuilder.Build(_ => _
      .InitDefaults(typeof(string))
      .SetId(4)
      .SetName(nameof(IStandardActor.ValueTaskTMethod))
      .SetInterfaceMethod(interfaceValueTaskTMethod)
      .SetImplementationMethod(implementationValueTaskTMethod)
      .SetParameterTypes(Type.EmptyTypes)
      .SetActivityType(typeof(StandardActorActivity4))
      .SetActivityFields(activity4Fields)
      .SetHasCancellationTokenParameter(false));

    ITaskMethodDescriptor handlerMethod = TaskMethodDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetId(8)
      .SetName(nameof(StandardActor.HandleAsync))
      .SetInterfaceMethod(null)
      .SetImplementationMethod(implementationHandlerMethod)
      .SetParameterTypes(new[] { typeof(ActorMessage), typeof(CancellationToken) })
      .SetActivityType(typeof(StandardActorActivity8))
      .SetActivityFields(activity8Fields)
      .SetHasCancellationTokenParameter(true));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(StandardActor)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStandardActor))
      .SetActorType(typeof(StandardActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetIsVirtual(false)
      .SetJsonSerializerOptions(jsonSerializerOptions)
      .SetActivityBaseType(typeof(StandardActorActivity))
      .SetMethods(taskMethod, taskTMethod, valueTaskMethod, valueTaskTMethod)
      .SetActivities(_ => _
        .Add(taskMethod)
        .Add(taskTMethod)
        .Add(valueTaskMethod)
        .Add(valueTaskTMethod)
        .Add<VoidMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(5)
          .SetName(nameof(StandardActor.VoidActivity))
          .SetInterfaceMethod(null)
          .SetImplementationMethod(voidActivityMethod)
          .SetParameterTypes(Type.EmptyTypes)
          .SetActivityType(typeof(StandardActorActivity5))
          .SetActivityFields(activity5Fields)
          .SetHasCancellationTokenParameter(false))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(6)
          .SetName(nameof(StandardActor.ActivityOverload))
          .SetInterfaceMethod(null)
          .SetImplementationMethod(activityOverload1Method)
          .SetParameterTypes(new[] { typeof(int) })
          .SetActivityType(typeof(StandardActorActivity6))
          .SetActivityFields(activity6Fields)
          .SetHasCancellationTokenParameter(false))
        .Add<TaskMethodDescriptorBuilder>(_ => _
          .InitDefaults()
          .SetId(7)
          .SetName(nameof(StandardActor.ActivityOverload))
          .SetInterfaceMethod(null)
          .SetImplementationMethod(activityOverload2Method)
          .SetParameterTypes(new[] { typeof(string) })
          .SetActivityType(typeof(StandardActorActivity7))
          .SetActivityFields(activity7Fields)
          .SetHasCancellationTokenParameter(false))
        .Add(handlerMethod))
      .SetId(_ => _
        .SetType(typeof(string))
        .SetHasIdSource(false)
        .SetStateIndex(-1))
      .SetState(_ => _
        .SetType(new StateTypeDelegator(typeof(StandardActorState)))
        .SetFields(stateFields)
        .Setup(mock => mock
          .Setup(x => x.GetDefaultValue())
          .Returns(() => new StandardActorState())))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IStandardActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod))
      .SetMessageHandlers(_ => _
        .Add(_ => _
          .SetInterfaceType(typeof(IMessageHandler<ActorMessage>))
          .SetMessageType(typeof(ActorMessage))
          .SetResultType(typeof(void))
          .SetInterfaceMethod(interfaceHandlerMethod)
          .SetActivity(handlerMethod)
          .SetHandlerMetadataCollection(new MessageHandlerMetadata("bus", "topic")))));
  }

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(StandardActor);
    Type activityBaseType = typeof(StandardActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), false))
      .Returns(Descriptor.State.Type);

    activityTypeBuilderMock
      .Setup(x => x.BuildBase(actorType))
      .Returns(activityBaseType);

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 1), actorType, activityBaseType))
      .Returns(typeof(StandardActorActivity1));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 2), actorType, activityBaseType))
      .Returns(typeof(StandardActorActivity2));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 3), actorType, activityBaseType))
      .Returns(typeof(StandardActorActivity3));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 4), actorType, activityBaseType))
      .Returns(typeof(StandardActorActivity4));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 5), actorType, activityBaseType))
      .Returns(typeof(StandardActorActivity5));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 6), actorType, activityBaseType))
      .Returns(typeof(StandardActorActivity6));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 7), actorType, activityBaseType))
      .Returns(typeof(StandardActorActivity7));

    activityTypeBuilderMock
      .Setup(x => x.Build(It.Is<IMethodDescriptor>(method => method.Id == 8), actorType, activityBaseType))
      .Returns(typeof(StandardActorActivity8));
  }

  public static void AssertValidDescriptor(IActorDescriptor descriptor)
  {
    descriptor.Should().BeAssignableTo<IActorDescriptor<StandardActor, StandardActorState>>();
    descriptor.Should().BeEquivalentTo(Descriptor, opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
  }
}
