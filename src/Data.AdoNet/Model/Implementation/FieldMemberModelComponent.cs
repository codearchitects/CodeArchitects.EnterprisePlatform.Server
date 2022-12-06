using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class FieldMemberModelComponent : AccessibleMemberModelComponent
{
  public FieldMemberModelComponent(FieldInfo field, Getter<object?> getValue, Setter<object?> setValue)
    : base(getValue, setValue)
  {
    Field = field;
  }

  public new FieldInfo Field { get; }

  public override MemberInfo Member => Field;

  protected override Type TypeCore => Field.FieldType;

  protected override FieldInfo FieldCore => Field;

  protected override PropertyInfo? PropertyCore => null;

  public static Getter<object?> BuildGetAccessor(FieldInfo field, string? memberName = null)
  {
    memberName ??= field.Name;
    DynamicMethod getMethod = new($"get_{memberName}", field.FieldType, Type.EmptyTypes, field.DeclaringType!);
    ILGenerator il = getMethod.GetILGenerator();

    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldfld, field);
    il.Emit(OpCodes.Ret);

    return (Getter<object?>)getMethod.CreateDelegate(typeof(Getter<object?>));
  }

  public static Setter<object?> BuildSetAccessor(FieldInfo field, string? memberName = null)
  {
    memberName ??= field.Name;
    DynamicMethod setMethod = new($"set_{memberName}", typeof(void), new[] { field.FieldType }, field.DeclaringType!);
    ILGenerator il = setMethod.GetILGenerator();

    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Stfld, field);
    il.Emit(OpCodes.Ret);

    return (Setter<object?>)setMethod.CreateDelegate(typeof(Setter<object?>));
  }

  public static FieldMemberModelComponent Create(FieldInfo field)
  {
    Getter<object?> getValue = BuildGetAccessor(field);
    Setter<object?> setValue = BuildSetAccessor(field);

    return new FieldMemberModelComponent(field, getValue, setValue);
  }
}
