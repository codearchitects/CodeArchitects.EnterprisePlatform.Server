using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit;

internal static class AssemblyBuilderExtensions
{
  private static readonly ConcurrentDictionary<ModuleBuilder, ConstructorInfo> s_attributeConstructors;

  static AssemblyBuilderExtensions()
  {
    s_attributeConstructors = new ConcurrentDictionary<ModuleBuilder, ConstructorInfo>();
  }

  public static void IgnoreAccessChecksTo(this ModuleBuilder module, string assemblyName)
  {
    ConstructorInfo constructor = s_attributeConstructors.GetOrAdd(module, GetAttributeConstructor);
    AssemblyBuilder assembly = (AssemblyBuilder)module.Assembly;

    assembly.SetCustomAttribute(new CustomAttributeBuilder(
      con: constructor,
      constructorArgs: new object[] { assemblyName }));
  }

  private static ConstructorInfo GetAttributeConstructor(ModuleBuilder module)
  {
    TypeBuilder type = module.DefineType(
      name: "System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute",
      attr: TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: typeof(Attribute));

    ConstructorInfo attributeUsageConstructor = typeof(AttributeUsageAttribute).GetConstructor(
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      binder: null,
      types: new[] { typeof(AttributeTargets) },
      modifiers: null) ?? throw new MissingMethodException(typeof(AttributeUsageAttribute).Name, ConstructorInfo.ConstructorName);

    PropertyInfo allowMultipleProperty = typeof(AttributeUsageAttribute).GetProperty(
      name: nameof(AttributeUsageAttribute.AllowMultiple),
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      binder: null,
      returnType: typeof(bool),
      types: Type.EmptyTypes,
      modifiers: null) ?? throw new MissingMemberException(typeof(AttributeUsageAttribute).Name, nameof(AttributeUsageAttribute.AllowMultiple));

    type.SetCustomAttribute(new CustomAttributeBuilder(
      con: attributeUsageConstructor,
      constructorArgs: new object[] { AttributeTargets.Assembly },
      namedProperties: new[] { allowMultipleProperty },
      propertyValues: new object[] { true }));

    FieldBuilder assemblyNameField = type.DefineField(
      fieldName: "<AssemblyName>k__BackingField",
      type: typeof(string),
      attributes: FieldAttributes.Private | FieldAttributes.InitOnly);

    ConstructorBuilder constructor = type.DefineConstructor(
      attributes: MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
      callingConvention: CallingConventions.Standard | CallingConventions.HasThis,
      parameterTypes: new[] { typeof(string) });

    ILGenerator constructorIL = constructor.GetILGenerator();
    constructorIL.Emit(OpCodes.Ldarg_0);                  // Push $this                                                            | Stack: $this
    constructorIL.Emit(OpCodes.Ldarg_1);                  // Push the first argument                                               | Stack: $this, $args[1]
    constructorIL.Emit(OpCodes.Stfld, assemblyNameField); // Store the first argument into the <AssemblyName>k__BackingField field | Stack: -
    constructorIL.Emit(OpCodes.Ret);                      // Return                                                                | Stack: -

    MethodBuilder assemblyNameGetter = type.DefineMethod(
      name: "get_AssemblyName",
      attributes: MethodAttributes.Public | MethodAttributes.SpecialName,
      returnType: typeof(string),
      parameterTypes: Type.EmptyTypes);

    ILGenerator assemblyNameGetterIL = assemblyNameGetter.GetILGenerator();
    assemblyNameGetterIL.Emit(OpCodes.Ldarg_0);                  // Push $this                  | Stack: $this
    assemblyNameGetterIL.Emit(OpCodes.Ldfld, assemblyNameField); // Load the assemblyName field | Stack: <AssemblyName>k__BackingField
    assemblyNameGetterIL.Emit(OpCodes.Ret);                      // Return                      | Stack: -

    PropertyBuilder assemblyNameProperty = type.DefineProperty(
      name: "AssemblyName",
      attributes: PropertyAttributes.None,
      returnType: typeof(string),
      parameterTypes: Type.EmptyTypes);

    assemblyNameProperty.SetGetMethod(assemblyNameGetter);

    return type.CreateTypeInfo()!.GetConstructor(
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      binder: null,
      types: new[] { typeof(string) },
      modifiers: null);
  }
}
