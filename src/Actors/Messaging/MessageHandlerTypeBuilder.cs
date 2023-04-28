using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Emit;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Messaging;

internal class MessageHandlerTypeBuilder
{
  public const string ComponentName = "MessageHandler";

  protected readonly ModuleBuilder _module;
  protected IILGeneratorProvider _ilProvider;

  protected MessageHandlerTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
  {
    _module = module;
    _ilProvider = ilProvider;
  }

  protected TypeBuilder DefineType(Type actorType, Type baseType)
  {
    return _module.DefineType(
      name: actorType.GetComponentTypeName(ComponentName),
      attr: TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: baseType);
  }

  protected MethodBuilder DefineMethodOverride(TypeBuilder type, IMessageHandlerDescriptor messageHandler)
  {
    type.AddInterfaceImplementation(messageHandler.InterfaceType);

    return type.DefineMethodOverrideFromDeclaration(messageHandler.InterfaceMethod, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final);
  }
}
