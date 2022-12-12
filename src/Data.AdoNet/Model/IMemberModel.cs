using CodeArchitects.Platform.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IMemberModel
{
  Type Type { get; }
  
  MemberInfo? Member { get; }
  
  FieldInfo? Field { get; }
  
  PropertyInfo? Property { get; }

  [MemberNotNullWhen(true, nameof(Member), nameof(GetValue), nameof(SetValue))]
  bool HasMember { get; }

  Getter<object?>? GetValue { get; }

  Setter<object?>? SetValue { get; }
}
