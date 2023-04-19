using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a model for a database entity.
/// </summary>
[Experimental]
public interface IEntityModel<TEntity, TKey> : IEntityModel
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// The primary key for this entity.
  /// </summary>
  new IPrimaryKeyModel<TKey> PrimaryKey { get; }
}
