using CodeArchitects.Platform.Data.AdoNet.Model.Builder;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class PropertyMemberComponent<T> : AccessibleMemberComponent<T>
{
  public PropertyMemberComponent(PropertyInfo property, Getter<T> getValue, Setter<T> setValue)
    : base(getValue, setValue)
  {
    Property = property;
  }

  public new PropertyInfo Property { get; }

  public override MemberInfo Member => Property;

  protected override Type TypeCore => Property.PropertyType;

  protected override FieldInfo? FieldCore => null;

  protected override PropertyInfo? PropertyCore => Property;

  public static Getter<T> BuildGetAccessor(PropertyInfo property)
  {
    Type entityType = property.DeclaringType!;

    if (property.GetMethod is null)
      throw new ModelConfigurationException($"Property '{property.Name}' on type '{entityType.Name}' does not have a getter.");

    DynamicMethod method = new($"getvalue_{property.Name}", typeof(T), new[] { typeof(object) }, entityType);
    ILGenerator il = method.GetILGenerator();

    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Castclass, entityType);
    il.Emit(OpCodes.Callvirt, property.GetMethod);
    if (typeof(T) == typeof(object) && property.PropertyType.IsValueType)
    {
      il.Emit(OpCodes.Box, property.PropertyType);
    }
    il.Emit(OpCodes.Ret);

    return (Getter<T>)method.CreateDelegate(typeof(Getter<T>));
  }

  public static Setter<T> BuildSetAccessor(PropertyInfo property)
  {
    Type entityType = property.DeclaringType!;

    if (property.SetMethod is null)
    {
      if (!property.TryGetBackingFieldByConvention(out FieldInfo? backingField))
        throw new ModelConfigurationException($"Property '{property.Name}' on type '{entityType.Name}' does not have a setter or a backing field resolvable by convention.");

      return FieldMemberComponent<T>.BuildSetAccessor(backingField, property.Name);
    }

    DynamicMethod method = new($"setvalue_{property.Name}", typeof(void), new[] { typeof(object), typeof(T) }, entityType);
    ILGenerator il = method.GetILGenerator();

    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Castclass, entityType);
    il.Emit(OpCodes.Ldarg_1);
    if (typeof(T) == typeof(object) && property.PropertyType.IsValueType)
    {
      il.Emit(OpCodes.Unbox_Any, property.PropertyType);
    }
    il.Emit(OpCodes.Callvirt, property.SetMethod);
    il.Emit(OpCodes.Ret);

    return (Setter<T>)method.CreateDelegate(typeof(Setter<T>));
  }

  public static PropertyMemberComponent<T> Create(PropertyInfo property)
  {
    Debug.Assert(typeof(T).Equals(property.PropertyType) || typeof(T) == typeof(object), "Invalid member component type.");

    Getter<T> getValue = BuildGetAccessor(property);
    Setter<T> setValue = BuildSetAccessor(property);

    return new(property, getValue, setValue);
  }
}
