using CodeArchitects.Platform.Data.AdoNet.Model.Builder;
using System.Diagnostics;
using System.Reflection;

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
    if (property.GetMethod is null)
      throw new ModelConfigurationException($"Property '{property.Name}' on type '{property.DeclaringType!.Name}' does not have a getter.");

    return (Getter<T>)Delegate.CreateDelegate(typeof(Getter<T>), property.GetMethod);
  }

  public static Setter<T> BuildSetAccessor(PropertyInfo property)
  {
    if (property.SetMethod is not null)
      return (Setter<T>)Delegate.CreateDelegate(typeof(Setter<T>), property.SetMethod);

    if (!property.TryGetBackingFieldByConvention(out FieldInfo? backingField))
      throw new ModelConfigurationException($"Property '{property.Name}' on type '{property.DeclaringType!.Name}' does not have a setter or a backing field resolvable by convention.");

    return FieldMemberComponent<T>.BuildSetAccessor(backingField, property.Name);
  }

  public static PropertyMemberComponent<T> Create(PropertyInfo property)
  {
    Debug.Assert(typeof(T).Equals(property.PropertyType), "Invalid member component type.");

    Getter<T> getValue = BuildGetAccessor(property);
    Setter<T> setValue = BuildSetAccessor(property);

    return new(property, getValue, setValue);
  }
}
