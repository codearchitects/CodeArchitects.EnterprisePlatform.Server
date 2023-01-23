using CodeArchitects.Platform.Common.CodeAnalysis;

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
  new ICollectionAccessor CollectionAccessor { get; }
}
