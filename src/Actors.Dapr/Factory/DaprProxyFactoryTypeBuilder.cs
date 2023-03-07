using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Factory;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Emit;
using Dapr.Actors.Client;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Dapr.Factory;

internal class DaprProxyFactoryTypeBuilder : ProxyFactoryTypeBuilder
{
  public DaprProxyFactoryTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
    : base(module, ilProvider)
  {
  }

  public Type Build(IActorDescriptor actor, IActorHostEmitResult hostEmitResult, Type proxyType, string? actorName)
  {
    Type baseType = typeof(ProxyFactory<,,,>).MakeGenericType(
      hostEmitResult.InterfaceType.UnderlyingSystemType,
      actor.Id.Type.UnderlyingSystemType,
      actor.InterfaceType.UnderlyingSystemType,
      actor.State.Type.UnderlyingSystemType);

    TypeBuilder type = BuildCore(actor, baseType);

    BuildConstructor(type);
    BuildActorNameProperty(type, actorName ?? actor.ActorType.Name);
    BuildCreateProxyMethod(type, proxyType, hostEmitResult.InterfaceType);
    BuildInitAsyncMethod(type, actor, hostEmitResult.InterfaceType);

    return type.CreateTypeInfo()!;
  }

  private void BuildConstructor(TypeBuilder type)
  {
    ConstructorInfo baseConstructor = type.BaseType!.GetRequiredConstructor(
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
      types: new[] { typeof(IActorProxyFactory) });

    ConstructorBuilder constructor = type.DefineConstructor(
      attributes: MethodAttributes.Public,
      callingConvention: CallingConventions.Standard,
      parameterTypes: new[] { typeof(IActorProxyFactory) });

    constructor.DefineParameter(1, ParameterAttributes.None, "actorFactory");

    IILGenerator il = _ilProvider.GetILGenerator(constructor);

    il.Emit(OpCodes.Ldarg_0);                 // Push $this                | Stack: $this
    il.Emit(OpCodes.Ldarg_1);                 // Push $actorFactory        | Stack: $this, $actorFactory
    il.Emit(OpCodes.Call, baseConstructor);   // Call base($actorFactory)  | Stack: -
    il.Emit(OpCodes.Ret);                     // Return                    | Stack: -
  }

  private void BuildActorNameProperty(TypeBuilder type, string actorName)
  {
    MethodInfo declaration = type.BaseType!.BaseType!.GetRequiredMethod(
      name: "get_ActorName",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
      types: Type.EmptyTypes);

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);
    IILGenerator il = _ilProvider.GetILGenerator(method);

    il.Emit(OpCodes.Ldstr, actorName); // Push literal actorName | Stack: actorName
    il.Emit(OpCodes.Ret);              // Return                 | Stack: actorName
  }

  private void BuildCreateProxyMethod(TypeBuilder type, Type proxyType, Type actorHostType)
  {
    ConstructorInfo proxyConstructor = proxyType.GetRequiredConstructor(
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { actorHostType.UnderlyingSystemType });

    MethodInfo declaration = type.BaseType!.GetRequiredMethod(
      name: "CreateProxy",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
      types: new[] { actorHostType.UnderlyingSystemType });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);
    IILGenerator il = _ilProvider.GetILGenerator(method);

    il.Emit(OpCodes.Ldarg_1);                       // Push $actorHost argument                | Stack: $actorHost
    il.Emit(OpCodes.Newobj, proxyConstructor);      // Create $proxy := new TProxy($actorHost) | Stack: $proxy 
    il.Emit(OpCodes.Ret);                           // Return                                  | Stack: $proxy
  }

  private void BuildInitAsyncMethod(TypeBuilder type, IActorDescriptor actor, Type hostInterfaceType)
  {
    Type stateType = actor.State.Type;

    MethodInfo declaration = type.BaseType!.GetRequiredMethod(
      name: "InitAsync",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
      types: new[] { hostInterfaceType.UnderlyingSystemType, stateType.UnderlyingSystemType, typeof(CancellationToken) });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);
    IILGenerator il = _ilProvider.GetILGenerator(method);

    if (actor.IsVirtual)
    {
      ConstructorInfo exceptionConstructor = typeof(NotSupportedException).GetRequiredConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: Type.EmptyTypes);

      il.Emit(OpCodes.Newobj, exceptionConstructor); // $exception := new NotSupportedException() | Stack: $exception
      il.Emit(OpCodes.Throw);                        // Throw                                     | Stack: $exception
    }
    else
    {
      MethodInfo initAsyncMethod = hostInterfaceType.GetRequiredMethod(
        name: Constants.InitAsyncMethodName,
        bindingAttr: BindingFlags.Public | BindingFlags.Instance,
        types: new[] { stateType.UnderlyingSystemType, typeof(CancellationToken) });

      il.Emit(OpCodes.Ldarg_1);                   // Push $actorHost                                   | Stack: $actorHost
      il.Emit(OpCodes.Ldarg_2);                   // Push $state                                       | Stack: $actorHost, $state
      il.Emit(OpCodes.Ldarg_3);                   // Push $ct                                          | Stack: $actorHost, $state, $ct
      il.Emit(OpCodes.Callvirt, initAsyncMethod); // Call $result := $actorHost.InitAsync($state, $ct) | Stack: $result
      il.Emit(OpCodes.Ret);                       // Return                                            | Stack: $result
    }
  }
}
