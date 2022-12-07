using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class FieldMemberComponent<T> : AccessibleMemberComponent<T>
{
  public FieldMemberComponent(FieldInfo field, Getter<T> getValue, Setter<T> setValue)
    : base(getValue, setValue)
  {
    Field = field;
  }

  public new FieldInfo Field { get; }

  public override MemberInfo Member => Field;

  protected override Type TypeCore => Field.FieldType;

  protected override FieldInfo FieldCore => Field;

  protected override PropertyInfo? PropertyCore => null;

  public static Getter<T> BuildGetAccessor(FieldInfo field, string? memberName = null)
  {
    memberName ??= field.Name;
    DynamicMethod getMethod = new($"get_{memberName}", field.FieldType, Type.EmptyTypes, field.DeclaringType!);
    ILGenerator il = getMethod.GetILGenerator();

    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldfld, field);
    il.Emit(OpCodes.Ret);

    return (Getter<T>)getMethod.CreateDelegate(typeof(Getter<T>));
  }

  public static Setter<T> BuildSetAccessor(FieldInfo field, string? memberName = null)
  {
    memberName ??= field.Name;
    DynamicMethod setMethod = new($"set_{memberName}", typeof(void), new[] { field.FieldType }, field.DeclaringType!);
    ILGenerator il = setMethod.GetILGenerator();

    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Stfld, field);
    il.Emit(OpCodes.Ret);

    return (Setter<T>)setMethod.CreateDelegate(typeof(Setter<T>));
  }

  public static FieldMemberComponent<T> Create(FieldInfo field)
  {
    Debug.Assert(typeof(T).Equals(field.FieldType), "Invalid member component type.");

    Getter<T> getValue = BuildGetAccessor(field);
    Setter<T> setValue = BuildSetAccessor(field);

    return new(field, getValue, setValue);
  }
}
