extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a simple navigation.
/// </summary>
/// <remarks>
/// A simple navigation is a navigation that does not require a junction table.
/// One-to-one associations and one-to-many associations are typically simple navigations.
/// </remarks>
[Experimental]
public interface ISimpleNavigationModel : INavigationModel
{
  /// <summary>
  /// Indicates whether this navigation property is on the dependent side of the relationship.
  /// </summary>
  /// <remarks>
  /// If <see langword="false"/>, the <see cref="NavigationEntity"/> property is not null.
  /// </remarks>
  [MemberNotNullWhen(false, nameof(NavigationEntity))]
  new bool IsOnDependent { get; } // TODO: Rename 'IsOnPrincipal' and change value accordingly

  /// <summary>
  /// The primary key of the entity on the dependent side of the relationship.
  /// </summary>
  IPrimaryKeyModel PrimaryKey { get; }

  /// <summary>
  /// The primary-foreign key pairs defining the navigation.
  /// </summary>
  IReadOnlyList<IKeyPair> KeyPairs { get; }

  /// <summary>
  /// The inverse navigation.
  /// </summary>
  new ISimpleNavigationModel Inverse { get; }

  /// <summary>
  /// A partial entity model of the dependent entity which contains the foreign keys only.
  /// </summary>
  IEntityModel? NavigationEntity { get; }
}
