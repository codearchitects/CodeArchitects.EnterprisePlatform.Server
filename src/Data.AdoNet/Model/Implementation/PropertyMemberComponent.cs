using CodeArchitects.Platform.Data.AdoNet.Model.Builder;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class PropertyMemberComponent : AccessibleMemberComponent
{
  public PropertyMemberComponent(PropertyInfo property, Getter<object?> getValue, Setter<object?> setValue)
    : base(getValue, setValue)
  {
    Property = property;
  }

  public new PropertyInfo Property { get; }

  public override MemberInfo Member => Property;

  protected override Type TypeCore => Property.PropertyType;

  protected override FieldInfo? FieldCore => null;

  protected override PropertyInfo? PropertyCore => Property;

  public static Getter<object?> BuildGetAccessor(PropertyInfo property)
  {
    if (property.GetMethod is null)
      throw new ModelConfigurationException($"Property '{property.Name}' on type '{property.DeclaringType!.Name}' does not have a getter.");

    return (Getter<object?>)Delegate.CreateDelegate(typeof(Getter<object?>), property.GetMethod);
  }

  public static Setter<object?> BuildSetAccessor(PropertyInfo property)
  {
    if (property.SetMethod is not null)
      return (Setter<object?>)Delegate.CreateDelegate(typeof(Setter<object?>), property.SetMethod);

    if (!property.TryGetBackingFieldByConvention(out FieldInfo? backingField))
      throw new ModelConfigurationException($"Property '{property.Name}' on type '{property.DeclaringType!.Name}' does not have a setter or a backing field resolvable by convention.");

    return FieldMemberComponent.BuildSetAccessor(backingField, property.Name);
  }

  public static PropertyMemberComponent Create(PropertyInfo property)
  {
    Getter<object?> getValue = BuildGetAccessor(property);
    Setter<object?> setValue = BuildSetAccessor(property);

    return new PropertyMemberComponent(property, getValue, setValue);
  }
}
