extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a skip navigation.
/// </summary>
/// <remarks>
/// A skip navigation is a navigation that requires a junction table.
/// Many-to-many associations are typically skip navigations.
/// </remarks>
[Experimental]
public interface ISkipNavigationModel : INavigationModel
{
  /// <summary>
  /// The junction entity in the many-to-many relationship.
  /// </summary>
  IEntityModel JunctionEntity { get; }

  /// <summary>
  /// The key pairs defining the navigation from the "from" entity to the junction entity.
  /// </summary>
  IReadOnlyList<IKeyPair> FromKeyPairs { get; }

  /// <summary>
  /// The key pairs defining the navigation from the junction entity to the "to" entity.
  /// </summary>
  IReadOnlyList<IKeyPair> ToKeyPairs { get; }

  /// <summary>
  /// The inverse navigation.
  /// </summary>
  new ISkipNavigationModel Inverse { get; }

  /// <summary>
  /// Creates a junction object for the given "from" and "to" objects.
  /// </summary>
  /// <param name="from">The object on the "from" side of the relationship.</param>
  /// <param name="to">The object on the "to" side of the relationship.</param>
  /// <returns>An instance of the junction entity.</returns>
  object CreateJunction(object from, object to);
}
