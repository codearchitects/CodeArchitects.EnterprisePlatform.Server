using CodeArchitects.Platform.Emit;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal class StateTypeBuilder : IStateTypeBuilder
{
  public const string ComponentName = "State";

  private readonly ModuleBuilder _module;
  private readonly IILGeneratorProvider _ilProvider;

  public StateTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
  {
    _module = module;
    _ilProvider = ilProvider;
  }

  public Type Build(Type actorType, IEnumerable<FieldInfo> stateFields, bool isPolymorphic)
  {
    Debug.Assert(stateFields.Count() > 0 || isPolymorphic, "Expected at least one state component or a polymorphic actor.");

    TypeBuilder type = _module.DefineType(
      name: actorType.GetComponentTypeName(ComponentName),
      attr: TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class);

    foreach (FieldInfo stateField in stateFields)
    {
      BuildAutoProperty(type, stateField.Name, stateField.FieldType);
    }

    if (isPolymorphic)
    {
      BuildAutoProperty(type, "$discriminator", typeof(string));
    }

    return type.CreateTypeInfo()!;
  }

  private PropertyInfo BuildAutoProperty(TypeBuilder type, string propertyName, Type propertyType)
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

    return property;
  }
}
