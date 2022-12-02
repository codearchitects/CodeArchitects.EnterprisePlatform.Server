using CodeArchitects.Platform.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPropertyModelBase
{
  Type Type { get; }
  
  MemberInfo? Member { get; }
  
  FieldInfo? Field { get; }
  
  PropertyInfo? Property { get; }

  [MemberNotNullWhen(true, nameof(Member), nameof(Getter), nameof(Setter))]
  bool HasMember { get; }

  Getter<object?>? Getter { get; }

  Setter<object?>? Setter { get; }
}
