using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Common.Collections;
using CodeArchitects.Platform.Emit;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class ImplementationFactoryTypeBuilder
{
  public const string ComponentName = "ImplementationFactory";

  private readonly ModuleBuilder _module;
  private readonly IILGeneratorProvider _ilProvider;

  public ImplementationFactoryTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
  {
    _module = module;
    _ilProvider = ilProvider;
  }

  public Type Build(IActorDescriptor actor)
  {
    Type actorType = actor.ActorType;
    Type stateType = actor.State.StateType;
    Type interfaceType = typeof(IImplementationFactory<,>).MakeGenericType(
      actorType.UnderlyingSystemType,
      stateType.UnderlyingSystemType);

    TypeBuilder type = _module.DefineType(
      name: actorType.GetComponentTypeName(ComponentName),
      attr: TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: typeof(object),
      interfaces: new[] { interfaceType });

    FieldInfo servicesField = DefineServicesField(type);
    BuildConstructor(type, servicesField);
    BuildCreateMethod(type, servicesField, actor);

    return type.CreateTypeInfo()!;
  }

  private static FieldInfo DefineServicesField(TypeBuilder type)
  {
    return type.DefineField(
      fieldName: "_services",
      type: typeof(IServiceProvider),
      attributes: FieldAttributes.Private | FieldAttributes.InitOnly);
  }

  private ConstructorInfo BuildConstructor(TypeBuilder type, FieldInfo servicesField)
  {
    ConstructorBuilder constructor = type.DefineConstructor(
      attributes: MethodAttributes.Public,
      callingConvention: CallingConventions.Standard,
      parameterTypes: new[] { typeof(IServiceProvider) });

    constructor.DefineParameter(
      iSequence: 1,
      attributes: ParameterAttributes.None,
      strParamName: "services");

    ConstructorInfo objectConstructor = typeof(object).GetRequiredConstructor();

    IILGenerator il = _ilProvider.GetILGenerator(constructor);

    il.Emit(OpCodes.Ldarg_0);                 // Push $this                         | Stack: $this
    il.Emit(OpCodes.Call, objectConstructor); // Call object constructor            | Stack: -
    il.Emit(OpCodes.Ldarg_0);                 // Push $this                         | Stack: $this
    il.Emit(OpCodes.Ldarg_1);                 // Push $services                     | Stack: $this, $services
    il.Emit(OpCodes.Stfld, servicesField);    // Store $services into servicesField | Stack: -
    il.Emit(OpCodes.Ret);                     // Return                             | Stack: -

    return constructor;
  }

  private MethodInfo BuildCreateMethod(TypeBuilder type, FieldInfo servicesField, IActorDescriptor actor)
  {
    Type actorType = actor.ActorType;
    Type stateType = actor.State.StateType;
    Type interfaceType = typeof(IImplementationFactory<,>).MakeGenericType(
      actorType.UnderlyingSystemType,
      stateType.UnderlyingSystemType);

    Type[] parameterTypes = new[]
    {
      typeof(IActorContextProvider<>).MakeGenericType(actorType.UnderlyingSystemType),
      stateType.UnderlyingSystemType
    };

    MethodInfo methodDeclaration = interfaceType.GetRequiredMethod(
      name: nameof(IImplementationFactory<object, object>.Create),
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: parameterTypes);

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(
      declaration: methodDeclaration,
      attributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final);

    IILGenerator il = _ilProvider.GetILGenerator(method);

    CreateMethodImplementor.Implement(actor, il, servicesField);

    return method;
  }

  private class CreateMethodImplementor : IDependencyDescriptorVisitor
  {
    private static readonly MethodInfo s_getRequiredServiceMethod = typeof(ServiceProviderServiceExtensions).GetRequiredMethod(
      name: nameof(ServiceProviderServiceExtensions.GetRequiredService),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IServiceProvider) });

    private static readonly MethodInfo s_getServiceMethod = typeof(ServiceProviderServiceExtensions).GetRequiredMethod(
      name: nameof(ServiceProviderServiceExtensions.GetService),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IServiceProvider) });

    private static readonly MethodInfo s_opEqualityMethod = typeof(string).GetRequiredMethod(
      name: "op_Equality",
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(string), typeof(string) });

    private static readonly MethodInfo s_concatMethod = typeof(string).GetRequiredMethod(
      name: nameof(string.Concat),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(string), typeof(string), typeof(string) });

    private static readonly ConstructorInfo s_invalidOperationExceptionConstructor = typeof(InvalidOperationException).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    private readonly IILGenerator _il;
    private readonly MethodInfo _getContextMethod;
    private readonly FieldInfo _servicesField;
    private readonly FieldInfo[] _stateTypeFields;

    private CreateMethodImplementor(IILGenerator il, MethodInfo getContextMethod, FieldInfo servicesField, FieldInfo[] stateTypeFields)
    {
      _il = il;
      _getContextMethod = getContextMethod;
      _servicesField = servicesField;
      _stateTypeFields = stateTypeFields;
    }

    public void VisitContextDependency(IContextDependencyDescriptor dependency)
    {
      MethodInfo getContextMethod = _getContextMethod.MakeGenericMethod(dependency.ImplementationType);

      _il.Emit(OpCodes.Ldarg_1);                    // Push $contextProvider                                           | Stack: ...dependencies, $contextProvider
      _il.Emit(OpCodes.Callvirt, getContextMethod); // Call $context := $contextProvider.GetContext<TImplementation>() | Stack: ...dependencies, $context
    }

    public void VisitServiceDependency(IServiceDependencyDescriptor dependency)
    {
      MethodInfo getServiceMethod = dependency.IsOptional
        ? s_getServiceMethod.MakeGenericMethod(dependency.Type.UnderlyingSystemType)
        : s_getRequiredServiceMethod.MakeGenericMethod(dependency.Type.UnderlyingSystemType);

      _il.Emit(OpCodes.Ldarg_0);                // Push $this                                             | Stack: ...dependencies, $this
      _il.Emit(OpCodes.Ldfld, _servicesField);  // Load $this._servicesField                              | Stack: ...dependencies, _stateField
      _il.Emit(OpCodes.Call, getServiceMethod); // Call $service := _servicesField.GetService<TService>() | Stack: ...dependencies, $service
    }

    public void VisitStateDependency(IStateDependencyDescriptor dependency)
    {
      _il.Emit(OpCodes.Ldarg_2);                                        // Push $state                                             | Stack: ...dependencies, $state
      _il.Emit(OpCodes.Ldfld, _stateTypeFields[dependency.FieldIndex]); // Load $stateField := $state._stateTypeFields[FieldIndex] | Stack: ...dependencies, $stateField
    }

    public static void Implement(IActorDescriptor actor, IILGenerator il, FieldInfo servicesField)
    {
      FieldInfo[] stateTypeFields = actor.State.StateType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
      MethodInfo getContextMethod = typeof(IActorContextProvider<>).MakeGenericType(actor.ActorType.UnderlyingSystemType).GetRequiredMethod(
        name: nameof(IActorContextProvider<object>.GetContext),
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: Type.EmptyTypes);

      if (!actor.IsPolymorphic)
      {
        Implement(actor.BaseImplementation);
        return;
      }

      IReadOnlyList<IImplementationDescriptor> implementations = actor.Implementations;
      FieldInfo? discriminatorField = actor.State.DiscriminatorField;
      Debug.Assert(discriminatorField is not null, "A polymorphic actor's state must have the discriminator field.");

      il.DeclareLocal(typeof(string));
      ILabel[] labels = Enumerable.Range(0, implementations.Count + 1).Map(_ => il.DefineLabel());

      il.Emit(OpCodes.Ldarg_2);                    // Push $state                                 | Stack: $state
      il.Emit(OpCodes.Ldfld, discriminatorField!); // Load $discriminator := $state.discriminator | Stack: $discriminator
      il.Emit(OpCodes.Stloc_0);                    // Store $discriminator in local variable      | Stack: -

      for (int i = 0; i < implementations.Count; i++)
      {
        IImplementationDescriptor implementation = implementations[i];

        il.Emit(OpCodes.Ldloc_0);                             // Load $discriminator                                   | Stack: $discriminator
        il.Emit(OpCodes.Ldstr, implementation.Type.FullName); // Push $fullName := the implementation type's full name | Stack: $discriminator, $fullName
        il.Emit(OpCodes.Call, s_opEqualityMethod);            // Compare $areEqual := $discriminator == $fullName      | Stack: $areEqual
        il.Emit(OpCodes.Brtrue_S, labels[i]);                 // If $areEqual, jump to *CREATE_IMPLEMENTATION[i]*      | Stack: -
      }

      il.Emit(OpCodes.Br, labels[^1]); // Jump to *INVALID_DISCRIMINATOR* | Stack: -

      for (int i = 0; i < implementations.Count; i++)
      {
        il.MarkLabel(labels[i]);       // *CREATE_IMPLEMENTATION[i]*                 | Stack: -
        Implement(implementations[i]); // Return the created implementation instance | Stack: -
      }

      il.MarkLabel(labels[^1]);                                        // *INVALID_DISCRIMINATOR*                                     | Stack: -
      il.Emit(OpCodes.Ldstr, "Invalid actor discriminator: '");        // Push $str1 := the first part of the error message           | Stack: $str1
      il.Emit(OpCodes.Ldloc_0);                                        // Load $discriminator                                         | Stack: $str1, $discriminator
      il.Emit(OpCodes.Ldstr, "'.");                                    // Push $str2 := the second part of the error message          | Stack: $str1, $discriminator, $str2
      il.Emit(OpCodes.Call, s_concatMethod);                           // Call $message = string.Concat($str1, $discriminator, $str2) | Stack: $message
      il.Emit(OpCodes.Newobj, s_invalidOperationExceptionConstructor); // Create $exception = new InvalidOperationException($message) | Stack: $exception
      il.Emit(OpCodes.Throw);                                          // Throw                                                       | Stack: -

      void Implement(IImplementationDescriptor implementation)
      {
        ConstructorInfo constructor = implementation.Constructor.Constructor;

        CreateMethodImplementor implementor = new(il, getContextMethod, servicesField, stateTypeFields);

        foreach (IDependencyDescriptor dependency in implementation.Constructor.Dependencies)
        {
          dependency.Accept(implementor);
        }

        il.Emit(OpCodes.Newobj, constructor); // Create $actor := new TActor(...dependencies); | Stack: $actor
        il.Emit(OpCodes.Ret);                 // Return                                        | Stack: $actor
      }
    }
  }
}
