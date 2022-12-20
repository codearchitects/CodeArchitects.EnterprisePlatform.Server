using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a navigation in an entity model.
/// </summary>
[Experimental]
public interface INavigationModel : IMemberModel
{
  /// <summary>
  /// The unique identifier for this navigation property.
  /// </summary>
  int Id { get; }

  /// <summary>
  /// The kind of association between the two entities.
  /// </summary>
  AssociationKind AssociationKind { get; }

  /// <summary>
  /// Indicates whether this navigation property is on the dependent side of the relationship.
  /// </summary>
  bool IsOnDependent { get; } // TODO: Rename 'IsOnPrincipal' and change value accordingly

  /// <summary>
  /// Indicates whether this navigation property represents a collection of entities.
  /// </summary>
  bool IsCollection { get; }

  /// <summary>
  /// The entity model on the "from" side of the relationship.
  /// </summary>
  IEntityModel From { get; }

  /// <summary>
  /// The entity model on the "to" side of the relationship.
  /// </summary>
  IEntityModel To { get; }

  /// <summary>
  /// The inverse navigation.
  /// </summary>
  INavigationModel Inverse { get; }

  /// <summary>
  /// The kind of collection represented by this navigation property, if it represents a collection.
  /// </summary>
  CollectionKind CollectionKind { get; }

  /// <summary>
  /// An object that provides access to the elements in the collection represented by this navigation property, if it represents an accessible collection.
  /// </summary>
  ICollectionAccessor? CollectionAccessor { get; }
}
