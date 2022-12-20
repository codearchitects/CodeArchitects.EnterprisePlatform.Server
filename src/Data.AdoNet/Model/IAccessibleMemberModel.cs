using CodeArchitects.Platform.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents an accessible member (field or property) of an entity.
/// </summary>
[Experimental]
public interface IAccessibleMemberModel : IMemberModel
{
  /// <summary>
  /// The <see cref="MemberInfo"/> object representing this member.
  /// </summary>
  new MemberInfo Member { get; }

  /// <summary>
  /// A delegate that can be used to get the value of this member for a given object instance.
  /// </summary>
  new Getter<object?> GetValue { get; }

  /// <summary>
  /// A delegate that can be used to set the value of this member for a given object instance.
  /// </summary>
  new Setter<object?> SetValue { get; }
}
