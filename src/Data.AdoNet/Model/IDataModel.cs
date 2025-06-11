extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// The database model. Contains metadata on how the entities map to the database and the relationships between them.
/// </summary>
[Experimental]
public interface IDataModel
{
  /// <summary>
  /// The entities in the database.
  /// </summary>
  IReadOnlyCollection<IEntityModel> Entities { get; }

  /// <summary>
  /// Tries to get the entity with the specified type.
  /// </summary>
  /// <param name="entityType">The type of the entity to get.</param>
  /// <param name="entity">The entity, if found, <c>null</c> otherwise.</param>
  /// <returns>True if the entity was found, false otherwise.</returns>
  bool TryGetEntity(Type entityType, [NotNullWhen(true)] out IEntityModel? entity);

  /// <summary>
  /// Tries to get the entity with the specified type and primary key type.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity to get.</typeparam>
  /// <typeparam name="TKey">The type of the primary key of the entity.</typeparam>
  /// <param name="entity">The entity, if found, <c>null</c> otherwise.</param>
  /// <returns>True if the entity was found, false otherwise.</returns>
  bool TryGetEntity<TEntity, TKey>([NotNullWhen(true)] out IEntityModel<TEntity, TKey>? entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
