using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IPropertyModelBase
{
  string Name { get; }
  
  Type Type { get; }
  
  MemberInfo? Member { get; }
  
  FieldInfo? Field { get; }
  
  PropertyInfo? Property { get; }

  [MemberNotNullWhen(true, nameof(Member), nameof(Accessor))]
  bool HasMember { get; }
  
  IAccessor Accessor { get; }
}
