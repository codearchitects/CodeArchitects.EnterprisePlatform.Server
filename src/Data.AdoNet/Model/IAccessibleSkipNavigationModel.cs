extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents an accessible skip navigation.
/// </summary>
/// <remarks>
/// A skip navigation is a navigation that requires a junction table.
/// Many-to-many associations are typically skip navigations.
/// </remarks>
[Experimental]
public interface IAccessibleSkipNavigationModel : ISkipNavigationModel, IAccessibleNavigationModel
{
  /// <summary>
  /// An object that provides access to the elements in the collection represented by this navigation.
  /// </summary>
  new ICollectionAccessor CollectionAccessor { get; }
}
