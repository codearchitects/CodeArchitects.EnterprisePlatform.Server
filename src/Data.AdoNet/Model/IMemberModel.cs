using CodeArchitects.Platform.Common.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a member (field or property) of an entity.
/// The member can be "hidden" if it is not defined on the type, but it is inferred from the relationships with other entities (e.g., an hidden foreign key).
/// </summary>
[Experimental]
public interface IMemberModel
{
  /// <summary>
  /// The type of this member.
  /// </summary>
  Type Type { get; }

  /// <summary>
  /// The <see cref="MemberInfo"/> object representing this member.
  /// </summary>
  MemberInfo? Member { get; }

  /// <summary>
  /// The <see cref="FieldInfo"/> object representing this member, if it is a field.
  /// </summary>
  FieldInfo? Field { get; }

  /// <summary>
  /// The <see cref="PropertyInfo"/> object representing this member, if it is a property.
  /// </summary>
  PropertyInfo? Property { get; }

  /// <summary>
  /// If true, the entity defines the member represented by this model, otherwise the member is "hidden".
  /// </summary>
  /// <remarks>
  /// If <c>true</c>, the <see cref="Member"/>, <see cref="GetValue"/>, and <see cref="SetValue"/> properties are not null.
  /// </remarks>
  [MemberNotNullWhen(true, nameof(Member), nameof(GetValue), nameof(SetValue))]
  bool HasMember { get; }

  /// <summary>
  /// A delegate that can be used to get the value of this member for a given object instance.
  /// </summary>
  Getter<object?>? GetValue { get; }

  /// <summary>
  /// A delegate that can be used to set the value of this member for a given object instance.
  /// </summary>
  Setter<object?>? SetValue { get; }

  /// <summary>
  /// The default value of this member.
  /// </summary>
  object? DefaultValue { get; }
}
