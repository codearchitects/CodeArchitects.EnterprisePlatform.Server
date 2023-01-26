using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal class PropertyReflectionStateFieldMetadata : ReflectionStateFieldMetadata
{
  private readonly PropertyInfo _property;

  public PropertyReflectionStateFieldMetadata(FieldInfo field, PropertyInfo property)
    : base(field)
  {
    _property = property;
  }

  protected override MemberInfo Member => _property;

  protected override Type MemberType => _property.PropertyType;
}
