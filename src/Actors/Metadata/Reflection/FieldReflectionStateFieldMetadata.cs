using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal class FieldReflectionStateFieldMetadata : ReflectionStateFieldMetadata
{
  public FieldReflectionStateFieldMetadata(FieldInfo field)
    : base(field)
  {
  }

  protected override MemberInfo Member => Field;

  protected override Type MemberType => Field.FieldType;
}
