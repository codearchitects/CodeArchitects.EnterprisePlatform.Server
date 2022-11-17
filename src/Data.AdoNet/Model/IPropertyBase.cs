using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IPropertyBase
{
  Type Type { get; }
  FieldInfo? Field { get; }
  PropertyInfo? Property { get; }
  MemberAccess MemberAccess { get; }
}
