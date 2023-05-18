using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Emit;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

internal class ActorHostTypeBuilder
{
  public const string InterfaceComponentName = "IActor";
  public const string HostComponentName = "Host";
  public const string HandlerInterfaceComponentName = "IMessageHandler";

  private readonly ModuleBuilder _module;
  private readonly IILGeneratorProvider _ilProvider;

  public ActorHostTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
  {
    _module = module;
    _ilProvider = ilProvider;
  }

  public ActorHostEmitResult Build(IActorDescriptor actor, string? actorName)
  {
    Type baseType = typeof(DaprActorHost<,>).MakeGenericType(
      actor.ActorType.UnderlyingSystemType,
      actor.State.Type.UnderlyingSystemType);

    MethodInfo actorGetter = baseType.GetRequiredMethod(
      name: "get_Actor",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: Type.EmptyTypes);

    TypeBuilder interfaceType = _module.DefineType(
      name: actor.ActorType.GetComponentTypeName(InterfaceComponentName),
      attr: TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface,
      parent: null,
      interfaces: new[] { typeof(IActor) });

    TypeBuilder hostType = _module.DefineType(
      name: actor.ActorType.GetComponentTypeName(HostComponentName),
      attr: TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: baseType,
      interfaces: new[] { interfaceType });

    BuildActorAttribute(hostType, actorName ?? actor.BaseImplementation.Type.Name);
    BuildConstructor(hostType, actor);

    IReadOnlyCollection<IMethodDescriptor> methods = actor.Methods;

    Dictionary<IMethodDescriptor, int> overloadSuffixes = new();
    IEnumerable<IGrouping<string, IMethodDescriptor>> groups = methods.GroupBy(method => method.Name);

    foreach (var group in groups)
    {
      if (group.Count() == 1)
        continue;

      int suffix = 1;
      foreach (IMethodDescriptor method in group)
      {
        overloadSuffixes.Add(method, suffix++);
      }
    }

    Dictionary<IMethodDescriptor, MethodInfo> methodDictionary = new();
    MethodNameProvider methodNameProvider = delegate (IMethodDescriptor descriptor)
    {
      return overloadSuffixes.TryGetValue(descriptor, out int suffix)
        ? $"{descriptor.Name}-{suffix}"
        : descriptor.Name;
    };
    ActorMethodBuilder methodBuilder = new(_ilProvider, interfaceType, hostType, actorGetter, methodNameProvider, methodDictionary);

    foreach (IMethodDescriptor method in methods)
    {
      method.Accept(methodBuilder);
    }

    if (!actor.IsVirtual)
    {
      BuildInitAsyncMethod(interfaceType, hostType);
    }

    Type? handlerInterfaceType = actor.MessageHandlers.Count > 0
      ? BuildAndAddHandlerInterfaceType(hostType, actorGetter, actor)
      : null;

    return new ActorHostEmitResult(interfaceType.CreateTypeInfo()!, hostType.CreateTypeInfo()!, handlerInterfaceType, methodDictionary);
  }

  private void BuildActorAttribute(TypeBuilder type, string actorName)
  {
    Type attributeType = typeof(global::Dapr.Actors.Runtime.ActorAttribute);
    ConstructorInfo attributeConstructor = attributeType.GetRequiredConstructor();
    PropertyInfo typeNameProperty = typeof(global::Dapr.Actors.Runtime.ActorAttribute).GetRequiredProperty(
      name: nameof(global::Dapr.Actors.Runtime.ActorAttribute.TypeName),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public);

    type.SetCustomAttribute(new CustomAttributeBuilder(attributeConstructor, Array.Empty<object>(), new[] { typeNameProperty }, new[] { actorName }));
  }

  private void BuildConstructor(TypeBuilder hostType, IActorDescriptor actor)
  {
    Type managerFactoryType = typeof(IManagerFactory<,>).MakeGenericType(
      actor.ActorType.UnderlyingSystemType,
      actor.State.Type.UnderlyingSystemType);

    Type[] parameterTypes = new[] { typeof(ActorHost), managerFactoryType };

    ConstructorInfo baseConstructor = hostType.BaseType!.GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: parameterTypes);

    ParameterInfo[] parameters = baseConstructor.GetParameters();

    ConstructorBuilder constructor = hostType.DefineConstructor(
      attributes: MethodAttributes.Public,
      callingConvention: CallingConventions.Standard,
      parameterTypes: parameterTypes);

    foreach (ParameterInfo parameter in parameters)
    {
      constructor.DefineParameter(parameter.Position + 1, ParameterAttributes.None, parameter.Name);
    }

    IILGenerator il = _ilProvider.GetILGenerator(constructor);

    il.Emit(OpCodes.Ldarg_0);               // Push $this                           | Stack: $this
    il.Emit(OpCodes.Ldarg_1);               // Push $host                           | Stack: $this, $host
    il.Emit(OpCodes.Ldarg_2);               // Push $factory                        | Stack: $this, $host, $factory 
    il.Emit(OpCodes.Call, baseConstructor); // Call base($host, $manager, $factory) | Stack: -
    il.Emit(OpCodes.Ret);                   // Return                               | Stack: -
  }

  private void BuildInitAsyncMethod(TypeBuilder interfaceType, TypeBuilder classType)
  {
    Type[] parameterTypes = new[] { typeof(byte[]), typeof(CancellationToken) };
    MethodInfo coreMethod = classType.BaseType!.GetRequiredMethod(
      name: "InitAsync",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: parameterTypes);

    Type returnType = typeof(Task);

    const string methodName = Constants.InitAsyncMethodName;
    MethodBuilder declaration = interfaceType.DefineMethod(
      name: methodName,
      attributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract,
      callingConvention: CallingConventions.HasThis,
      returnType: returnType,
      parameterTypes: parameterTypes);

    MethodBuilder method = classType.DefineMethod(
      name: methodName,
      attributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final,
      callingConvention: CallingConventions.HasThis,
      returnType: returnType,
      parameterTypes: parameterTypes);

    foreach (ParameterInfo parameter in coreMethod.GetParameters())
    {
      declaration.DefineParameter(parameter.Position + 1, parameter.Attributes, parameter.Name);
      method.DefineParameter(parameter.Position + 1, parameter.Attributes, parameter.Name);
    }

    classType.DefineMethodOverride(method, declaration);

    IILGenerator il = _ilProvider.GetILGenerator(method);

    il.Emit(OpCodes.Ldarg_0);          // Push $this                             | Stack: $this
    il.Emit(OpCodes.Ldarg_1);          // Push $state                            | Stack: $this, $state
    il.Emit(OpCodes.Ldarg_2);          // Push $ct                               | Stack: $this, $state, $ct
    il.Emit(OpCodes.Call, coreMethod); // Call $result := InitAsync($state, $ct) | Stack: $result
    il.Emit(OpCodes.Ret);              // Return                                 | Stack: $result
  }

  private Type BuildAndAddHandlerInterfaceType(TypeBuilder hostType, MethodInfo actorGetter, IActorDescriptor actor)
  {
    Type actorType = actor.ActorType;

    TypeBuilder handlerInterfaceType = _module.DefineType(
      name: actorType.GetComponentTypeName(HandlerInterfaceComponentName),
      attr: TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface,
      parent: null,
      interfaces: new[] { typeof(IActor) });

    hostType.AddInterfaceImplementation(handlerInterfaceType);

    int handlerIndex = 1;
    MethodNameProvider methodNameProvider = _ => $"HandleAsync-{handlerIndex++}";

    ActorMethodBuilder methodBuilder = new(_ilProvider, handlerInterfaceType, hostType, actorGetter, methodNameProvider, new());
    foreach (IMessageHandlerDescriptor messageHandler in actor.MessageHandlers)
    {
      messageHandler.Activity.Accept(methodBuilder);
    }

    return handlerInterfaceType.CreateTypeInfo()!;
  }

  private delegate string MethodNameProvider(IMethodDescriptor descriptor);

  private sealed class ActorMethodBuilder : IMethodDescriptorVisitor
  {
    private readonly IILGeneratorProvider _ilProvider;
    private readonly TypeBuilder _interfaceType;
    private readonly TypeBuilder _hostType;
    private readonly MethodInfo _actorGetter;
    private readonly MethodNameProvider _methodNameProvider;
    private readonly Dictionary<IMethodDescriptor, MethodInfo> _methodDictionary;

    public ActorMethodBuilder(IILGeneratorProvider ilProvider, TypeBuilder interfaceType, TypeBuilder hostType, MethodInfo actorGetter, MethodNameProvider methodNameProvider, Dictionary<IMethodDescriptor, MethodInfo> methodDictionary)
    {
      _ilProvider = ilProvider;
      _interfaceType = interfaceType;
      _hostType = hostType;
      _actorGetter = actorGetter;
      _methodNameProvider = methodNameProvider;
      _methodDictionary = methodDictionary;
    }

    public void VisitTaskMethod(ITaskMethodDescriptor descriptor)
    {
      VisitTaskMethodCore(descriptor);
    }

    public void VisitTaskTMethod(ITaskTMethodDescriptor descriptor)
    {
      VisitTaskMethodCore(descriptor);
    }

    public void VisitValueTaskMethod(IValueTaskMethodDescriptor descriptor)
    {
      VisitValueTaskMethodCore(descriptor, typeof(Task));
    }

    public void VisitValueTaskTMethod(IValueTaskTMethodDescriptor descriptor)
    {
      VisitValueTaskMethodCore(descriptor, typeof(Task<>).MakeGenericType(descriptor.ResultType.UnderlyingSystemType));
    }

    public void VisitVoidMethod(IVoidMethodDescriptor descriptor)
    {
      Debug.Fail("Interface methods should not be void.");
      throw Errors.Unreachable;
    }

    private void VisitTaskMethodCore(IMethodDescriptor descriptor)
    {
      int parameterCount = descriptor.ParameterTypes.Length;
      MethodInfo implementationMethod = descriptor.ImplementationMethod;
      MethodBuilder method = DefineStandardMethod(descriptor, descriptor.ReturnType);
      IILGenerator il = _ilProvider.GetILGenerator(method);

      il.Emit(OpCodes.Ldarg_0);                        // Push $this                           | Stack: $this
      il.Emit(OpCodes.Call, _actorGetter);             // Load $actor := $this.Actor           | Stack: $actor
      il.LoadArgs(parameterCount);                     // Push method arguments                | Stack: $actor, ...args
      il.Emit(OpCodes.Callvirt, implementationMethod); // Call $task := $actor.method(...args) | Stack: $task
      il.Emit(OpCodes.Ret);                            // Return                               | Stack: $task
    }

    private void VisitValueTaskMethodCore(IMethodDescriptor descriptor, Type returnType)
    {
      Type valueTaskType = descriptor.ReturnType;
      MethodInfo asTaskMethod = valueTaskType.GetRequiredMethod(
        name: nameof(ValueTask<object>.AsTask),
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: Type.EmptyTypes);

      int parameterCount = descriptor.ParameterTypes.Length;
      MethodInfo implementationMethod = descriptor.ImplementationMethod;
      MethodBuilder method = DefineStandardMethod(descriptor, returnType);
      IILGenerator il = _ilProvider.GetILGenerator(method);

      il.DeclareLocal(valueTaskType);

      il.Emit(OpCodes.Ldarg_0);                        // Push $this                                | Stack: $this
      il.Emit(OpCodes.Call, _actorGetter);             // Load $actor := $this.Actor                | Stack: $actor
      il.LoadArgs(parameterCount);                     // Push method arguments                     | Stack: $actor, ...args
      il.Emit(OpCodes.Callvirt, implementationMethod); // Call $valueTask := $actor.method(...args) | Stack: $valueTask
      il.Emit(OpCodes.Stloc_0);                        // Store $valueTask inside local             | Stack: -
      il.Emit(OpCodes.Ldloca_S, 0);                    // Load $valueTask from local                | Stack: $valueTask
      il.Emit(OpCodes.Call, asTaskMethod);             // Call $task := $valueTask.AsTask()         | Stack: $task
      il.Emit(OpCodes.Ret);                            // Return                                    | Stack: $task
    }

    private MethodBuilder DefineStandardMethod(IMethodDescriptor descriptor, Type returnType)
    {
      string methodName = _methodNameProvider(descriptor);

      MethodBuilder declaration = _interfaceType.DefineMethod(
        name: methodName,
        attributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract,
        callingConvention: CallingConventions.HasThis,
        returnType: returnType,
        parameterTypes: descriptor.ParameterTypes);

      MethodBuilder method = _hostType.DefineMethod(
        name: methodName,
        attributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final,
        callingConvention: CallingConventions.HasThis,
        returnType: returnType,
        parameterTypes: descriptor.ParameterTypes);

      foreach (ParameterInfo parameter in descriptor.ImplementationMethod.GetParameters())
      {
        declaration.DefineParameter(parameter.Position + 1, parameter.Attributes, parameter.Name);
        method.DefineParameter(parameter.Position + 1, parameter.Attributes, parameter.Name);
      }

      _hostType.DefineMethodOverride(method, declaration);
      _methodDictionary.Add(descriptor, declaration);

      return method;
    }
  }
}
