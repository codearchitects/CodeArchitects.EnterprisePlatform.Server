using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Emit;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Factory;

internal abstract class ProxyFactoryTypeBuilder
{
  public const string ComponentName = "ActorFactory";

  protected readonly ModuleBuilder _module;
  protected readonly IILGeneratorProvider _ilProvider;

  protected ProxyFactoryTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
  {
    _module = module;
    _ilProvider = ilProvider;
  }

  protected TypeBuilder BuildCore(IActorDescriptor actor, Type baseType)
  {
    TypeBuilder type = _module.DefineType(
      name: actor.ActorType.GetComponentTypeName(ComponentName),
      attr: TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: baseType,
      interfaces: new[] { actor.Factory.FactoryType });

    BuildCreateAsyncMethod(type, actor);
    BuildGetMethod(type, actor);

    return type;
  }

  private void BuildCreateAsyncMethod(TypeBuilder type, IActorDescriptor actor)
  {
    IActorFactoryDescriptor factory = actor.Factory;
    if (factory.CreateAsyncMethod is not MethodInfo declaration)
      return;

    IStateDescriptor state = actor.State;
    IActorIdDescriptor id = actor.Id;

    IReadOnlyList<FieldInfo> stateFields = state.Fields;
    ConstructorInfo stateConstructor = state.Type.GetRequiredConstructor();
    MethodInfo createCoreAsyncMethod = type.BaseType!.GetRequiredMethod(
      name: "CreateCoreAsync",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic)
      .MakeGenericMethod(id.Type.UnderlyingSystemType);

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final);
    IILGenerator il = _ilProvider.GetILGenerator(method);

    il.Emit(OpCodes.Ldarg_0);                     // Push $this                                                 | Stack: $this
    LoadId(il, id, out int argOffset);            // Load $id                                                   | Stack: $this, $id
    il.Emit(OpCodes.Newobj, stateConstructor);    // new TState()                                               | Stack: $this, $id, state
    for (int i = 0; i < stateFields.Count; i++)   //                                                            |
    {                                             //                                                            |
      il.Emit(OpCodes.Dup);                       // Duplicate state                                            | Stack: $this, $id, state, state
      il.LoadArg(i + argOffset);                  // Push $arg := $args[i + argOffset]                          | Stack: $this, $id, state, state, $arg
      il.Emit(OpCodes.Stfld, stateFields[i]);     // Assign the value of $arg to the i-th field of the state    | Stack: $this, $id, state
    }                                             //                                                            |
    il.LoadArg(stateFields.Count + argOffset);    // Push $ct                                                   | Stack: $this, $id, state, $ct
    il.Emit(OpCodes.Call, createCoreAsyncMethod); // Call $result := CreateCoreAsync<TActorId>($id, state, $ct) | Stack: $result
    il.Emit(OpCodes.Ret);                         // Return                                                     | Stack: $result

    static void LoadId(IILGenerator il, IActorIdDescriptor id, out int argOffset)
    {
      if (!id.HasIdSource)
      {
        il.Emit(OpCodes.Ldarg_1); // Push $id

        argOffset = 2;
        return;
      }

      int stateDependencyIndex = id.StateIndex;

      if (id.GetActorIdMethod is { } getActorIdMethod)
      {
        if (getActorIdMethod.DeclaringType.IsValueType)
        {
          il.Emit(OpCodes.Ldarga_S, stateDependencyIndex + 1); // Push $idDependency
          il.Emit(OpCodes.Call, getActorIdMethod);             // Get $id := $idDependency.GetActorId()
        }
        else
        {
          il.LoadArg(stateDependencyIndex + 1);        // Push $idDependency
          il.Emit(OpCodes.Callvirt, getActorIdMethod); // Get $id := $idDependency.GetActorId()
        }
      }
      else
      {
        il.LoadArg(stateDependencyIndex + 1); // Push $idDependency
      }

      argOffset = 1;
    }
  }

  private void BuildGetMethod(TypeBuilder type, IActorDescriptor actor)
  {
    IActorFactoryDescriptor factory = actor.Factory;

    MethodInfo coreMethod = type.BaseType!.GetRequiredMethod(
      name: "GetCore",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance)
      .MakeGenericMethod(actor.Id.Type.UnderlyingSystemType);

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(factory.GetMethod, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final);
    IILGenerator il = _ilProvider.GetILGenerator(method);

    il.Emit(OpCodes.Ldarg_0);          // Push $this                             | Stack: $this
    il.Emit(OpCodes.Ldarg_1);          // Push $id argument                      | Stack: $this, $id
    il.Emit(OpCodes.Call, coreMethod); // Call $result := GetCore<TActorId>($id) | Stack: $result
    il.Emit(OpCodes.Ret);              // Return                                 | Stack: $result
  }
}
