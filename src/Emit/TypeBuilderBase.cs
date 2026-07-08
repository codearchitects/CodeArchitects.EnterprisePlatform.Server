using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Emit;

internal class TypeBuilderBase
{
  protected readonly ModuleBuilder _module;
  protected readonly IILGeneratorProvider _ilProvider;

  public TypeBuilderBase(ModuleBuilder module, IILGeneratorProvider ilProvider)
  {
    _module = module;
    _ilProvider = ilProvider;
  }

  protected AutoPropertyInfo BuildAutoProperty(TypeBuilder type, string propertyName, Type propertyType)
  {
    FieldBuilder backingField = type.DefineField(
      fieldName: $"<{propertyName}>k__BackingField",
      type: propertyType,
      attributes: FieldAttributes.Assembly);

    MethodBuilder getter = type.DefineMethod(
      name: $"get_{propertyName}",
      attributes: MethodAttributes.Public | MethodAttributes.Final,
      callingConvention: CallingConventions.HasThis,
      returnType: propertyType,
      parameterTypes: Type.EmptyTypes);

    MethodBuilder setter = type.DefineMethod(
      name: $"set_{propertyName}",
      attributes: MethodAttributes.Public | MethodAttributes.Final,
      callingConvention: CallingConventions.HasThis,
      returnType: typeof(void),
      parameterTypes: new[] { propertyType });

    setter.DefineParameter(
      position: 1,
      attributes: ParameterAttributes.None,
      strParamName: "value");

    PropertyBuilder property = type.DefineProperty(
      name: propertyName,
      attributes: PropertyAttributes.None,
      callingConvention: CallingConventions.HasThis,
      returnType: propertyType,
      parameterTypes: Type.EmptyTypes);

    property.SetGetMethod(getter);
    property.SetSetMethod(setter);

    IILGenerator getterIL = _ilProvider.GetILGenerator(getter);

    getterIL.Emit(OpCodes.Ldarg_0);             // Push $this             | Stack: $this
    getterIL.Emit(OpCodes.Ldfld, backingField); // Push the backing field | Stack: backingField
    getterIL.Emit(OpCodes.Ret);                 // Return                 | Stack: backingField

    IILGenerator setterIL = _ilProvider.GetILGenerator(setter);

    setterIL.Emit(OpCodes.Ldarg_0);             // Push $this                           | Stack: $this
    setterIL.Emit(OpCodes.Ldarg_1);             // Push the value passed to the setter  | Stack: $this, $value
    setterIL.Emit(OpCodes.Stfld, backingField); // Store $value into the backing field  | Stack: -
    setterIL.Emit(OpCodes.Ret);                 // Return                               | Stack: -

    return new(property, backingField);
  }
}
