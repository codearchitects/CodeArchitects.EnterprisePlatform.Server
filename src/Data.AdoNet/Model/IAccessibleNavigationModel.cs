using CodeArchitects.Platform.Common.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents an accessible navigation in an entity model.
/// </summary>
[Experimental]
public interface IAccessibleNavigationModel : INavigationModel, IAccessibleMemberModel
{
  /// <summary>
  /// Indicates whether this navigation property represents a collection of entities.
  /// </summary>
  /// <remarks>
  /// If <see langword="true"/>, the <see cref="CollectionAccessor"/> property is not null.
  /// </remarks>
  [MemberNotNullWhen(true, nameof(CollectionAccessor))]
  new bool IsCollection { get; }

  /// <summary>
  /// An object that provides access to the elements in the collection represented by this navigation property, if it represents an accessible collection.
  /// </summary>
  new ICollectionAccessor? CollectionAccessor { get; }
}
