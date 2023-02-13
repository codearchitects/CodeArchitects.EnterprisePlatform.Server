using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Emit;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Scheduling;

internal class ActivityTypeBuilder : TypeBuilderBase
{
  public const string ComponentName = "Activity";

  private static readonly MethodInfo s_idGetter = typeof(Activity).GetRequiredMethod(
    name: $"get_{nameof(Activity.Id)}",
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: Type.EmptyTypes);
  private static readonly MethodInfo s_implementationIdGetter = typeof(Activity).GetRequiredMethod(
    name: $"get_{nameof(Activity.ImplementationId)}",
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: Type.EmptyTypes);

  public ActivityTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
    : base(module, ilProvider)
  {
  }

  public Type BuildBase(Type actorType)
  {
    Type baseType = typeof(Activity<>).MakeGenericType(actorType.UnderlyingSystemType);

    TypeBuilder type = _module.DefineType(
      name: actorType.GetComponentTypeName(ComponentName),
      attr: TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Class,
      parent: baseType);

    return type.CreateTypeInfo()!;
  }

  public Type Build(IMethodDescriptor descriptor, Type actorType, Type baseType)
  {
    MethodInfo implementationMethod = descriptor.ImplementationMethod;
    
    TypeBuilder type = _module.DefineType(
      name: actorType.GetComponentTypeName($"{ComponentName}{descriptor.Id}"),
      attr: TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: baseType);

    ImplementIdProperty(type, descriptor.Id);

    List<FieldInfo> argumentFields = new();
    foreach (ParameterInfo parameter in implementationMethod.GetParameters())
    {
      if (parameter.ParameterType == typeof(CancellationToken))
        continue;

      AutoPropertyInfo autoProperty = BuildAutoProperty(type, parameter.Name, parameter.ParameterType);
      argumentFields.Add(autoProperty.BackingField);
    }

    ImplementExecuteAsyncMethod(type, actorType, descriptor, argumentFields);

    return type.CreateTypeInfo()!;
  }

  private void ImplementIdProperty(TypeBuilder type, int id)
  {
    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(s_idGetter, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final);

    IILGenerator il = _ilProvider.GetILGenerator(method);

    il.LoadInt(id);       // Push id | Stack: id
    il.Emit(OpCodes.Ret); // Return  | Stack: id
  }

  private void ImplementExecuteAsyncMethod(TypeBuilder type, Type actorType, IMethodDescriptor descriptor, IReadOnlyList<FieldInfo> argumentFields)
  {
    MethodInfo implementationMethod = descriptor.ImplementationMethod;
    Type implementationType = implementationMethod.DeclaringType;

    MethodInfo declaration = typeof(Activity<>).MakeGenericType(actorType.UnderlyingSystemType).GetRequiredMethod(
      name: nameof(Activity<object>.ExecuteAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { actorType.UnderlyingSystemType, typeof(CancellationToken) });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final);

    IILGenerator il = _ilProvider.GetILGenerator(method);

    il.Emit(OpCodes.Ldarg_1);                           // Push $actor                        | Stack: $actor
    if (actorType != implementationType)                //                                    |
    {                                                   //                                    |
      il.Emit(OpCodes.Castclass, implementationType);   // Cast $actor to implementation type | Stack: $actor
    }                                                   //                                    |
                                                        //                                    |
    foreach (FieldInfo argumentField in argumentFields) //                                    |
    {                                                   //                                    |
      il.Emit(OpCodes.Ldarg_0);                         // Push $this                         | Stack: $actor, ...$args, $this
      il.Emit(OpCodes.Ldfld, argumentField);            // Load argumentField                 | Stack: $actor, ...$args, argumentField
    }                                                   //                                    |
                                                        //                                    |
    if (descriptor.HasCancellationTokenParameter)       //                                    |
    {                                                   //                                    |
      il.Emit(OpCodes.Ldarg_2);                         // Push $ct                           | Stack: $actor, ...$args, $ct
    }                                                   //                                    |
                                                        //                                    |
    descriptor.Accept(new ExecuteCallImplementor(il));  // Call implementationMethod          | Stack: $task
    il.Emit(OpCodes.Ret);                               // Return                             | Stack: $task
  }

  private class ExecuteCallImplementor : IMethodDescriptorVisitor
  {
    private static readonly MethodInfo s_completedTaskGetter = typeof(Task).GetRequiredMethod(
      name: $"get_{nameof(Task.CompletedTask)}",
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: Type.EmptyTypes);

    private readonly IILGenerator _il;

    public ExecuteCallImplementor(IILGenerator il)
    {
      _il = il;
    }

    public void VisitVoidMethod(IVoidMethodDescriptor descriptor)
    {
      _il.Emit(OpCodes.Callvirt, descriptor.ImplementationMethod); // Call implementationMethod(...$args) | Stack: -
      _il.Emit(OpCodes.Call, s_completedTaskGetter);               // Load $task := Task.CompletedTask    | Stack: $task
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
      VisitValueTaskMethodCore(descriptor);
    }

    public void VisitValueTaskTMethod(IValueTaskTMethodDescriptor descriptor)
    {
      VisitValueTaskMethodCore(descriptor);
    }

    private void VisitTaskMethodCore(IMethodDescriptor descriptor)
    {
      _il.Emit(OpCodes.Callvirt, descriptor.ImplementationMethod); // Call $task := implementationMethod(...$args) | Stack: $task
    }

    private void VisitValueTaskMethodCore(IMethodDescriptor descriptor)
    {
      Type returnType = descriptor.ReturnType;
      MethodInfo asTaskMethod = returnType.GetRequiredMethod(
        name: nameof(ValueTask.AsTask),
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: Type.EmptyTypes);

      _il.DeclareLocal(returnType);

      _il.Emit(OpCodes.Callvirt, descriptor.ImplementationMethod); // Call $valueTask _= implementationMethod(...$args) | Stack: $valueTask
      _il.Emit(OpCodes.Stloc_0);                                   // Store $valueTask inside local                     | Stack: -
      _il.Emit(OpCodes.Ldloca_S, 0);                               // Load $valueTask from local                        | Stack: $valueTask
      _il.Emit(OpCodes.Call, asTaskMethod);                        // Call $task := $valueTask.AsTask()                 | Stack: $task
    }
  }
}
