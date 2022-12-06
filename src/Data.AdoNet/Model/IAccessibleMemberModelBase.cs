using CodeArchitects.Platform.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IAccessibleMemberModelBase : IMemberModelBase
{
  new MemberInfo Member { get; }

  new Getter<object?> GetValue { get; }

  new Setter<object?> SetValue { get; }
}
