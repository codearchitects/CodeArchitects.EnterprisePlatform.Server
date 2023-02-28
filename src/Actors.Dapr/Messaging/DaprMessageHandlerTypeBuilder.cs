using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Messaging;
using CodeArchitects.Platform.Emit;
using Dapr.Actors.Client;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Dapr.Messaging;

internal class DaprMessageHandlerTypeBuilder : MessageHandlerTypeBuilder
{
  public DaprMessageHandlerTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
    : base(module, ilProvider)
  {
  }

  public Type Build(IActorDescriptor actor, Type handlerInterfaceType, string? actorName)
  {
    Type idType = actor.Id.Type;

    Type baseType = typeof(ActorHostFactory<,>).MakeGenericType(
      handlerInterfaceType,
      idType.UnderlyingSystemType);

    TypeBuilder type = DefineType(actor.ActorType, baseType, Type.EmptyTypes);

    BuildMessageHandlerAttribute(type);
    BuildHandlerConstructor(type);
    BuildActorNameProperty(type, actorName ?? actor.ActorType.Name);
    BuildHandlerMethods(type, handlerInterfaceType, idType, actor);

    return type.CreateTypeInfo()!;
  }

  private void BuildHandlerConstructor(TypeBuilder type)
  {
    ConstructorInfo baseConstructor = type.BaseType!.GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: new[] { typeof(IActorProxyFactory) });

    ConstructorBuilder constructor = type.DefineConstructor(
      attributes: MethodAttributes.Public,
      callingConvention: CallingConventions.Standard,
      parameterTypes: new[] { typeof(IActorProxyFactory) });

    constructor.DefineParameter(1, ParameterAttributes.None, "hostFactory");

    IILGenerator il = _ilProvider.GetILGenerator(constructor);

    il.Emit(OpCodes.Ldarg_0);               // Push $this            | Stack: $this
    il.Emit(OpCodes.Ldarg_1);               // Push $hostFactory     | Stack: $this, $hostFactory
    il.Emit(OpCodes.Call, baseConstructor); // Call base constructor | Stack: -
    il.Emit(OpCodes.Ret);                   // Return                | Stack: -
  }

  private void BuildActorNameProperty(TypeBuilder type, string actorName)
  {
    MethodInfo declaration = type.BaseType!.GetRequiredMethod(
      name: "get_ActorName",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
      types: Type.EmptyTypes);

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);
    IILGenerator il = _ilProvider.GetILGenerator(method);

    il.Emit(OpCodes.Ldstr, actorName); // Push literal actorName | Stack: actorName
    il.Emit(OpCodes.Ret);              // Return                 | Stack: actorName
  }

  private void BuildHandlerMethods(TypeBuilder type, Type handlerInterfaceType, Type idType, IActorDescriptor actor)
  {
    MethodInfo createHostMethod = type.BaseType!.GetRequiredMethod(
      name: "CreateHost",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: new[] { idType.UnderlyingSystemType });

    MethodInfo actorIdGetter = typeof(IActorMessage<>)
      .MakeGenericType(idType.UnderlyingSystemType)
      .GetRequiredMethod(
        name: $"get_{nameof(IActorMessage<object>.ActorId)}",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: Type.EmptyTypes);

    MethodInfo[] hostMethods = handlerInterfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

    foreach (IMessageHandlerDescriptor messageHandler in actor.MessageHandlers)
    {
      MethodInfo hostMethod = hostMethods.Single(method => method.GetParameters()[0].ParameterType == messageHandler.MessageType);

      MethodBuilder method = DefineMethodOverride(type, messageHandler);

      BuildMessageHandlerAttributes(method, messageHandler);

      IILGenerator il = _ilProvider.GetILGenerator(method);

      il.Emit(OpCodes.Ldarg_0);                 // Push $this                                       | Stack: $this
      il.Emit(OpCodes.Ldarg_1);                 // Push $message                                    | Stack: $this, $message
      il.Emit(OpCodes.Callvirt, actorIdGetter); // Get $id := $message.ActorId                      | Stack: $this, $id
      il.Emit(OpCodes.Call, createHostMethod);  // Call $host := CreateHost($id)                    | Stack: $host
      il.Emit(OpCodes.Ldarg_1);                 // Push $message                                    | Stack: $host, $message
      il.Emit(OpCodes.Ldarg_2);                 // Push $ct                                         | Stack: $host, $message, $ct
      il.Emit(OpCodes.Callvirt, hostMethod);    // Call $result := $host.HandleAsync($message, $ct) | Stack: $result
      il.Emit(OpCodes.Ret);                     // Return                                           | Stack: $result
    }
  }

  private void BuildMessageHandlerAttribute(TypeBuilder type)
  {
    type.SetCustomAttribute(new CustomAttributeBuilder(MessagingMetadata.Metadata.AttributeConstructor, new object?[] { null, null }));
  }

  private void BuildMessageHandlerAttributes(MethodBuilder method, IMessageHandlerDescriptor messageHandler)
  {
    foreach (IMessageHandlerMetadata metadata in messageHandler.HandlerMetadataCollection)
    {
      method.SetCustomAttribute(new CustomAttributeBuilder(MessagingMetadata.Metadata.AttributeConstructor, new object?[] { metadata.Bus, metadata.Topic }));
    }
  }
}
