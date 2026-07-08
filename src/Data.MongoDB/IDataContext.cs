using CodeArchitects.Platform.Common.CodeAnalysis;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Represents a <see cref="Data.IDataContext"/> that  is based on MongoDB.
/// </summary>
[Experimental]
public interface IDataContext : Data.IDataContext
{
  /// <summary>
  /// The MongoDB database <see cref="IMongoDatabase"/>.
  /// </summary>
  IMongoDatabase Database { get; }

  /// <summary>
  /// Gets the MongoDB collection based on the entity type.
  /// </summary>
  /// <typeparam name="TEntity">The MongoDB collection entity type.</typeparam>
  /// <returns>The MongoDB collection.</returns>
  IMongoCollection<TEntity> GetCollection<TEntity>()
    where TEntity : class;
}
