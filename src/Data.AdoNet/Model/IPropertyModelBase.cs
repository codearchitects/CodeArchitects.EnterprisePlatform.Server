using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IPropertyModelBase
{
  string Name { get; }
  Type Type { get; }
  MemberInfo? Member { get; }
  FieldInfo? Field { get; }
  PropertyInfo? Property { get; }
  MemberAccess MemberAccess { get; }
  IAccessor Accessor { get; }
}
