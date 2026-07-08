using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Emit;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Dapr.Proxy;

internal class ActorProxyTypeBuilder
{
  public const string ComponentName = "ActorProxy";

  private readonly ModuleBuilder _module;
  private readonly IILGeneratorProvider _ilProvider;

  public ActorProxyTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
  {
    _module = module;
    _ilProvider = ilProvider;
  }

  public Type Build(IActorDescriptor actor, IActorHostEmitResult hostEmitResult)
  {
    Type hostInterfaceType = hostEmitResult.InterfaceType;

    TypeBuilder type = _module.DefineType(
      name: actor.ActorType.GetComponentTypeName(ComponentName),
      attr: TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: null,
      interfaces: new[] { actor.InterfaceType.UnderlyingSystemType });

    FieldInfo actorHostField = DefineActorHostField(type, hostInterfaceType.UnderlyingSystemType);

    BuildConstructor(type, actorHostField);

    ProxyMethodBuilder methodBuilder = new(_ilProvider, type, hostEmitResult, actorHostField);
    foreach (IMethodDescriptor method in actor.Methods)
    {
      method.Accept(methodBuilder);
    }

    return type.CreateTypeInfo()!;
  }

  private static FieldInfo DefineActorHostField(TypeBuilder type, Type actorInterfaceType)
  {
    return type.DefineField(
      fieldName: "_actorHost",
      type: actorInterfaceType.UnderlyingSystemType,
      attributes: FieldAttributes.Private | FieldAttributes.InitOnly);
  }

  private void BuildConstructor(TypeBuilder type, FieldInfo actorHostField)
  {
    ConstructorBuilder constructor = type.DefineConstructor(
      attributes: MethodAttributes.Public,
      callingConvention: CallingConventions.Standard,
      parameterTypes: new[] { actorHostField.FieldType });

    constructor.DefineParameter(1, ParameterAttributes.None, "actorHost");

    IILGenerator il = _ilProvider.GetILGenerator(constructor);

    il.Emit(OpCodes.Ldarg_0);                           // Push $this                               | Stack: $this
    il.Emit(OpCodes.Call, EmitUtils.ObjectConstructor); // Call object constructor                  | Stack: -
    il.Emit(OpCodes.Ldarg_0);                           // Push $this                               | Stack: $this
    il.Emit(OpCodes.Ldarg_1);                           // Push $actorHost                          | Stack: $this, $actorHost
    il.Emit(OpCodes.Stfld, actorHostField);             // Store $actorHost inside $this._actorHost | Stack: -
    il.Emit(OpCodes.Ret);                               // Return                                   | Stack: -
  }

  private sealed class ProxyMethodBuilder : IMethodDescriptorVisitor
  {
    private readonly IILGeneratorProvider _ilProvider;
    private readonly TypeBuilder _type;
    private readonly IActorHostEmitResult _hostEmitResult;
    private readonly FieldInfo _actorHostField;

    public ProxyMethodBuilder(IILGeneratorProvider ilProvider, TypeBuilder type, IActorHostEmitResult hostEmitResult, FieldInfo actorHostField)
    {
      _ilProvider = ilProvider;
      _type = type;
      _hostEmitResult = hostEmitResult;
      _actorHostField = actorHostField;
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
      MethodInfo hostMethod = _hostEmitResult.GetHostMethod(descriptor);
      int parameterCount = descriptor.ParameterTypes.Length;

      MethodBuilder method = DefineProxyMethodOverride(descriptor);
      IILGenerator il = _ilProvider.GetILGenerator(method);

      il.Emit(OpCodes.Ldarg_0);                // Push $this                             | Stack: $this
      il.Emit(OpCodes.Ldfld, _actorHostField); // Load $host = $this._actorHost          | Stack: $host
      il.LoadArgs(parameterCount);             // Load ...$args                          | Stack: $host, ...$args
      il.Emit(OpCodes.Callvirt, hostMethod);   // Call $result := $host.Method(...$args) | Stack: $result
      il.Emit(OpCodes.Ret);                    // Return                                 | Stack: $result
    }

    private void VisitValueTaskMethodCore(IMethodDescriptor descriptor, Type taskType)
    {
      MethodInfo hostMethod = _hostEmitResult.GetHostMethod(descriptor);
      int parameterCount = descriptor.ParameterTypes.Length;
      ConstructorInfo resultConstructor = descriptor.ReturnType.GetRequiredConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: new[] { taskType });

      MethodBuilder method = DefineProxyMethodOverride(descriptor);
      IILGenerator il = _ilProvider.GetILGenerator(method);

      il.Emit(OpCodes.Ldarg_0);                   // Push $this                             | Stack: $this
      il.Emit(OpCodes.Ldfld, _actorHostField);    // Load $host = $this._actorHost          | Stack: $host
      il.LoadArgs(parameterCount);                // Load ...$args                          | Stack: $host, ...$args
      il.Emit(OpCodes.Callvirt, hostMethod);      // Call $task := $host.Method(...$args)   | Stack: $task
      il.Emit(OpCodes.Newobj, resultConstructor); // Create $result := new ValueTask($task) | Stack: $result
      il.Emit(OpCodes.Ret);                       // Return                                 | Stack: $result
    }

    private MethodBuilder DefineProxyMethodOverride(IMethodDescriptor descriptor)
    {
      Debug.Assert(descriptor.InterfaceMethod is not null, "Proxy methods must have a corresponding interface method.");

      return _type.DefineMethodOverrideFromDeclaration(descriptor.InterfaceMethod!, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final);
    }
  }
}
